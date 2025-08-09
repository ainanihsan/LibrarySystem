using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Infrastructure.Data
{
    public class DatabaseInitializer
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public DatabaseInitializer(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task InitializeSchemaAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var createBooksTable = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                Pages INTEGER NOT NULL,
                CopiesTotal INTEGER NOT NULL
            );";

            var createUsersTable = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Email TEXT NOT NULL UNIQUE
            );";

            var createLendingsTable = @"
            CREATE TABLE IF NOT EXISTS Lendings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                BookId INTEGER NOT NULL,
                UserId INTEGER NOT NULL,
                BorrowDate TEXT NOT NULL,
                ReturnDate TEXT,
                FOREIGN KEY(BookId) REFERENCES Books(Id),
                FOREIGN KEY(UserId) REFERENCES Users(Id)
            );";

            await connection.ExecuteAsync(createBooksTable);
            await connection.ExecuteAsync(createUsersTable);
            await connection.ExecuteAsync(createLendingsTable);
        }
    }

}
