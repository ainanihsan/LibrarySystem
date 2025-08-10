
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Application.Services;
using LibrarySystem.Shared.Infrastructure.Data;
using SQLitePCL;

var builder = WebApplication.CreateBuilder(args);

Batteries.Init();  // Initialize SQLite provider early

// Add services to the container.
builder.Services.AddGrpc();

// Register SQLite connection factory and repository
builder.Services.AddSingleton(new SqliteConnectionFactory("Data Source=library.db"));
builder.Services.AddTransient<IBookRepository,BookRepository>();
builder.Services.AddTransient<IBookService, BookService>();

// Register DatabaseInitializer and DatabaseSeeder
builder.Services.AddTransient<DatabaseInitializer>();
builder.Services.AddTransient<DatabaseSeeder>();



var app = builder.Build();

// Initialize database schema and seed data before handling requests
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await dbInitializer.InitializeSchemaAsync();

    var dbSeeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await dbSeeder.SeedAllAsync();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<BookGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
