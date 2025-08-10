using Dapper;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Infrastructure.Data
{
    public class BookRepository : IBookRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public BookRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Books>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT Id, Title, Author, Pages, CopiesTotal FROM Books";

            var books = await connection.QueryAsync<Books>(sql);
            return books;
        }

        public async Task<IEnumerable<(string,int)>> GetMostBorrowedBooks()
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT b.Title, COUNT(*) AS LendingCount " +
                "FROM Lendings l " +
                "INNER JOIN Books b ON l.BookId = b.Id " +
                "GROUP BY b.Title " +
                "ORDER BY LendingCount DESC LIMIT 2";

            var books = await connection.QueryAsync<(string,int)>(sql);
            return books;
        }


        public Task<Books?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

}
