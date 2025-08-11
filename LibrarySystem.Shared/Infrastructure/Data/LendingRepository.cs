using Dapper;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Infrastructure.Data
{
    public class LendingRepository : ILendingRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public LendingRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<LendingRecord>> GetBorrowRecordsForBook(int bookId)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "SELECT l.BorrowDate,l.ReturnDate, b.Pages from Lendings l " +
                        "INNER JOIN Books b ON b.Id = l.BookId " +
                        "WHERE b.Id = @bookId";

            var books = await connection.QueryAsync<LendingRecord>(sql,
                   new { bookId });
            return books;
        }
    }
}
