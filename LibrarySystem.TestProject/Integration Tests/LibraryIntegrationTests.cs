using Dapper;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Application.Services;
using LibrarySystem.Shared.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLitePCL;
using System.Data;


[TestClass]
public class BookIntegrationTests
{
    private static readonly string SharedConnString = $"Data Source=file:memdb_{Guid.NewGuid()}?mode=memory&cache=shared";

    private IDbConnection _masterConn;      // keep open for test life
    private SqliteConnectionFactory _connectionFactory;
    private IBookRepository _bookRepository;
    private ILendingRepository _lendingRepository;
    private IBookServices _bookService;

    [TestInitialize]
    public async Task Setup()
    {

        Batteries.Init();  // Initialize SQLite provider early

        _masterConn = new SqliteConnection(SharedConnString);
        _masterConn.Open();

            await _masterConn.ExecuteAsync(@"
        DROP TABLE IF EXISTS Lendings;
        DROP TABLE IF EXISTS Users;
        DROP TABLE IF EXISTS Books;
    ");

        // Create schema
        await _masterConn.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                Pages INTEGER NOT NULL,
                CopiesTotal INTEGER NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Lendings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId INTEGER NOT NULL,
                BookId INTEGER NOT NULL,
                BorrowDate TEXT NOT NULL,
                ReturnDate TEXT NULL,
                FOREIGN KEY(UserId) REFERENCES Users(Id),
                FOREIGN KEY(BookId) REFERENCES Books(Id)
            );
        ");

            // Clear existing data if any
            _masterConn.Execute("DELETE FROM Lendings;");
            _masterConn.Execute("DELETE FROM Users;");
            _masterConn.Execute("DELETE FROM Books;");

        // Seed books
        _masterConn.Execute(@"INSERT INTO Books (Title, Author, Pages, CopiesTotal) VALUES
            ('Brave New World', 'Aldous Huxley', 288, 4),
            ('The Catcher in the Rye', 'J.D. Salinger', 214, 6),
            ('Effective C#', 'Bill Wagner', 350, 3),
            ('Refactoring', 'Martin Fowler', 448, 2),
            ('Clean Architecture', 'Robert C. Martin', 432, 5);
        ");

        // Seed users
        _masterConn.Execute(@"INSERT INTO Users (Name, Email) VALUES
    ('Emily Clark', 'emilyc@example.com'),
    ('Frank Miller', 'frankm@example.com'),
    ('Grace Hopper', 'graceh@example.com'),
    ('Henry Ford', 'henryf@example.com'),
    ('Isabel Allende', 'isabela@example.com');
");

        // Seed lendings
        _masterConn.Execute(@"INSERT INTO Lendings (UserId, BookId, BorrowDate, ReturnDate) VALUES
    (1, 1, '2024-06-01', '2024-06-10'),
    (2, 2, '2024-06-05', NULL),
    (3, 3, '2024-06-08', '2024-06-18'),
    (3, 2, '2024-06-20', '2024-07-01'),
    (4, 4, '2024-07-01', '2024-07-20'),
    (5, 5, '2024-07-10', NULL),
    (1, 3, '2024-07-15', '2024-07-25');
");

        _connectionFactory = new SqliteConnectionFactory(SharedConnString);
        _lendingRepository = new LendingRepository(_connectionFactory);
        _bookRepository = new BookRepository(_connectionFactory);
        _bookService = new BookServices(_bookRepository, _lendingRepository);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _masterConn?.Dispose();
    }

    [TestMethod]
    public async Task GetAllBooksAsync_ReturnsSeededBooks()
    {
        var books = await _bookService.GetAllBooksAsync();

        Assert.IsNotNull(books);
        Assert.AreEqual(5, books.Count());
        Assert.IsTrue(books.Any(b => b.Title == "Brave New World"));
        Assert.IsTrue(books.Any(b => b.Title == "Refactoring"));
    }
    [TestMethod]
    public async Task GetAllBooksAsync_ReturnsAllSeededBooks()
    {
        var books = await _bookService.GetAllBooksAsync();

        Assert.IsNotNull(books);
        Assert.AreEqual(5, books.Count());
        Assert.IsTrue(books.Any(b => b.Title == "Brave New World"));
        Assert.IsTrue(books.Any(b => b.Title == "Refactoring"));
    }

    [TestMethod]
    public async Task GetMostBorrowedBooksAsync_ReturnsBooksOrderedByBorrowCount()
    {
        var result = await _bookService.GetMostBorrowedBooksAsync();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count() > 0);

        var ordered = result.OrderByDescending(b => b.Item2);
        CollectionAssert.AreEqual(ordered.ToList(), result.ToList(), "Books not ordered by borrow count desc");
    }

    [TestMethod]
    public async Task GetBookStats_ReturnsCorrectCopiesAndBorrowedCount()
    {
        // For book with Id=3 ("Effective C#")
        var stats = await _bookService.GetBookStats(3);

        // CopiesTotal = 3 from seed
        Assert.AreEqual(3, stats.Item1);

        // Borrowed count = 2 lendings in seed data for book id 3
        Assert.AreEqual(2, stats.Item2);
    }

    [TestMethod]
    public async Task CalculateReadingRateAsync_ReturnsCorrectPagesPerDay()
    {
        var rate = await _bookService.GetReadingEstimate(1); // BookId 1 - "Brave New World"

        // Manually calculated from seed:
        // Borrowing 1: 2024-06-01 to 2024-06-10 = 9 days, 288 pages
        // Reading rate = 288 / 9 = 32 pages/day
        Assert.IsTrue(rate > 30 && rate < 33, "Reading rate should be approx 32 pages/day");
    }

}
