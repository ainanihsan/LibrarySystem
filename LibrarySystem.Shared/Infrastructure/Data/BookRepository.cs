using Dapper;
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Infrastructure.Data
{
    public class BookRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public BookRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT Id, Title, Author, Pages, CopiesTotal FROM Books";

            var books = await connection.QueryAsync<Book>(sql);
            return books;
        }

    }

}
