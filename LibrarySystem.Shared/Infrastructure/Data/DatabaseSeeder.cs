using Dapper;
using System.Threading.Tasks;
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Infrastructure.Data
{
    public class DatabaseSeeder
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public DatabaseSeeder(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task SeedAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            // Seed Books
            var bookCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Books;");
            if (bookCount == 0)
            {
                var insertBooksSql = @"
                    INSERT INTO Books (Title, Author, Pages, CopiesTotal) VALUES
                    ('The Hobbit', 'J.R.R. Tolkien', 310, 5),
                    ('1984', 'George Orwell', 328, 3),
                    ('Clean Code', 'Robert C. Martin', 464, 2),
                    ('The Pragmatic Programmer', 'Andrew Hunt & David Thomas', 352, 4),
                    ('To Kill a Mockingbird', 'Harper Lee', 281, 3),
                    ('Brave New World', 'Aldous Huxley', 311, 2);
                ";
                await connection.ExecuteAsync(insertBooksSql);
            }

            // Seed Users
            var userCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Users;");
            if (userCount == 0)
            {
                var insertUsersSql = @"
                    INSERT INTO Users (Name, Email) VALUES
                    ('Alice Johnson', 'alice@example.com'),
                    ('Bob Smith', 'bob@example.com'),
                    ('Charlie Brown', 'charlie@example.com'),
                    ('Diana Prince', 'diana@example.com'),
                    ('Ethan Hunt', 'ethan@example.com');
                ";
                await connection.ExecuteAsync(insertUsersSql);
            }

            // Seed Lendings
            var lendingCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Lendings;");
            if (lendingCount == 0)
            {
                var insertLendingsSql = @"
                    INSERT INTO Lendings (BookId, UserId, BorrowDate, ReturnDate) VALUES
                    (1, 1, '2024-07-01', '2024-07-10'),
                    (2, 2, '2024-07-03', NULL),
                    (3, 3, '2024-07-05', '2024-07-15'),
                    (3, 2, '2024-07-13', '2024-07-31'),
                    (3, 1, '2024-08-05', '2024-08-15'),
                    (4, 4, '2024-08-01', NULL),
                    (5, 5, '2024-08-03', '2024-08-13'),
                    (1, 2, '2024-08-07', '2024-08-17'),
                    (2, 3, '2024-08-10', '2024-08-20'),
                    (6, 1, '2024-08-15', NULL);
                ";
                await connection.ExecuteAsync(insertLendingsSql);
            }
        }
    }
}
