using Dapper;
using LibrarySystem.GrpcClient;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Application.Services;
using LibrarySystem.Shared.Domain;
using LibrarySystem.Shared.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLitePCL;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
}

public class GetReadingEstimateResponseDto
{
    public double rate { get; set; }
}
public class MostBorrowedBookDto
{
    public string Title { get; set; }
    public int BorrowCount { get; set; }
}

public class BookStatsDto
{
    public int CopiesTotal { get; set; }
    public int BorrowedCount { get; set; }
}

public class TopUserDto
{
    public string Name { get; set; }
    public int Count { get; set; }
}

[TestClass]
public class LibrarySystemTests : IDisposable
{
    private static readonly string SharedConnString =
        $"Data Source=file:memdb_{Guid.NewGuid()}?mode=memory&cache=shared";

    private SqliteConnection _connection; // Keep open to keep DB alive
    private IHost _grpcHost;
    private WebApplicationFactory<Program> _factory;
    private HttpClient _httpClient;

    [TestInitialize]
    public async Task Setup()
    {
        Batteries.Init(); // SQLite initialization

        // Open SQLite in-memory connection (shared cache)
        _connection = new SqliteConnection(SharedConnString);
        await _connection.OpenAsync();

        // Create schema + seed data
        await _connection.ExecuteAsync(@"
            DROP TABLE IF EXISTS Lendings;
            DROP TABLE IF EXISTS Users;
            DROP TABLE IF EXISTS Books;

            CREATE TABLE Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                Pages INTEGER NOT NULL,
                CopiesTotal INTEGER NOT NULL
            );

            CREATE TABLE Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL
            );

            CREATE TABLE Lendings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                BookId INTEGER NOT NULL,
                BorrowDate TEXT NOT NULL,
                ReturnDate TEXT NULL,
                FOREIGN KEY(UserId) REFERENCES Users(Id),
                FOREIGN KEY(BookId) REFERENCES Books(Id)
            );
        ");

        await _connection.ExecuteAsync(@"
            INSERT INTO Books (Title, Author, Pages, CopiesTotal) VALUES
            ('Brave New World', 'Aldous Huxley', 288, 4),
            ('The Catcher in the Rye', 'J.D. Salinger', 214, 6),
            ('Effective C#', 'Bill Wagner', 350, 3),
            ('Refactoring', 'Martin Fowler', 448, 2),
            ('Clean Architecture', 'Robert C. Martin', 432, 5);
        ");

        await _connection.ExecuteAsync(@"
            INSERT INTO Users (Name, Email) VALUES
            ('Emily Clark', 'emilyc@example.com'),
            ('Frank Miller', 'frankm@example.com'),
            ('Grace Hopper', 'graceh@example.com'),
            ('Henry Ford', 'henryf@example.com'),
            ('Isabel Allende', 'isabela@example.com');
        ");

        await _connection.ExecuteAsync(@"
            INSERT INTO Lendings (UserId, BookId, BorrowDate, ReturnDate) VALUES
            (1, 1, '2024-06-01', '2024-06-10'),
            (2, 2, '2024-06-05', NULL),
            (3, 3, '2024-06-08', '2024-06-18'),
            (3, 2, '2024-06-20', '2024-07-01'),
            (4, 4, '2024-07-01', '2024-07-20'),
            (5, 5, '2024-07-10', NULL),
            (1, 3, '2024-07-15', '2024-07-25');
        ");

        // Start a real gRPC server on localhost:7165 for the API to call
        _grpcHost = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel()
                    .UseUrls("https://localhost:7165")
                    .ConfigureServices(services =>
                    {
                        services.AddGrpc();

                        // Register your SQLite factory using shared in-memory DB
                        services.AddSingleton(new SqliteConnectionFactory(SharedConnString));

                        // Register repositories and services as in your gRPC server project
                        services.AddTransient<IBookRepository, BookRepository>();
                        services.AddTransient<IUserRepository, UserRepository>();
                        services.AddTransient<ILendingRepository, LendingRepository>();

                        services.AddTransient<IBookServices, BookServices>();
                        services.AddTransient<IUserServices, UserServices>();

                        // Register your gRPC service implementations
                        services.AddTransient<BookGrpcService>();
                        services.AddTransient<UserGrpcService>();
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapGrpcService<BookGrpcService>();
                            endpoints.MapGrpcService<UserGrpcService>();
                        });
                    });
            })
            .Build();

        await _grpcHost.StartAsync();

        // Now start the API project in-memory with DI override for DB connection
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(SqliteConnectionFactory));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddSingleton(new SqliteConnectionFactory(SharedConnString));
                });
            });

        _httpClient = _factory.CreateClient();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        _httpClient?.Dispose();
        _factory?.Dispose();
        _connection?.Dispose();

        if (_grpcHost != null)
        {
            await _grpcHost.StopAsync();
            _grpcHost.Dispose();
        }
    }
    [TestMethod]
    public async Task GetAllBooks_ViaApi_CallsGrpcAndReturnsBooks()
    {
        var response = await _httpClient.GetAsync("/books");
        var jsonString = await response.Content.ReadAsStringAsync();
        Console.WriteLine(jsonString);


        var books = await _httpClient.GetFromJsonAsync<List<BookDto>>("/books");

        Assert.IsNotNull(books);
        Assert.AreEqual(5, books.Count);
        Assert.IsTrue(books.Any(b => b.Title == "Brave New World"));
        Assert.IsTrue(books.Any(b => b.Title == "Refactoring"));
    }

    [TestMethod]
    public async Task CalculateReadingRate_ViaApi_CallsGrpcAndReturnsExpectedRate()
    {
        var averageRate = await _httpClient.GetFromJsonAsync<double>("books/reading-estimate/1");
        Assert.IsTrue(averageRate > 30 && averageRate < 33);
    }


    [TestMethod]
    public async Task GetMostBorrowedBooks_ViaApi_ReturnsBooksInDescendingOrder()
    {
        var books = await _httpClient.GetFromJsonAsync<List<MostBorrowedBookDto>>("/books/most-borrowed");

        Assert.IsNotNull(books);
        Assert.IsTrue(books.SequenceEqual(books.OrderByDescending(b => b.BorrowCount)),
            "Books are not ordered by BorrowCount descending");
        Assert.IsTrue(books.Any(b => b.Title == "Effective C#"));
    }

    [TestMethod]
    public async Task GetBookStats_ViaApi_ReturnsCopiesTotalAndBorrowedCount()
    {
        int bookId = 1;
        var stats = await _httpClient.GetFromJsonAsync<BookStatsDto>($"/books/book-stats/{bookId}");

        Assert.IsNotNull(stats);
        Assert.AreEqual(4, stats.CopiesTotal);      // Adjust expected values to your seed data or mocks
        Assert.AreEqual(1, stats.BorrowedCount);
    }

    [TestMethod]
    public async Task GetTopUsers_ViaApi_ReturnsUsersWithinDateRange()
    {
        string start = "2024-06-01";
        string end = "2024-07-01";

        var users = await _httpClient.GetFromJsonAsync<List<TopUserDto>>($"/users/top-users?startDate={start}&endDate={end}");

        Assert.IsNotNull(users);
        Assert.AreEqual(3, users.Count);
        Assert.AreEqual("Emily Clark", users.First().Name);
        Assert.AreEqual(1, users.First().Count);
    }

    [TestMethod]
    public async Task GetUserBorrowedBooks_ViaApi_ReturnsBooksForUserWithinDateRange()
    {
        int userId = 1;
        string start = "2024-06-01";
        string end = "2024-07-1";

        var borrowedBooks = await _httpClient.GetFromJsonAsync<List<BookDto>>($"/users/user-books?id={userId}&startDate={start}&endDate={end}");

        Assert.IsNotNull(borrowedBooks);
        Assert.AreEqual(1, borrowedBooks.Count);
    }




    public void Dispose()
    {
        Cleanup().GetAwaiter().GetResult();
    }
}
