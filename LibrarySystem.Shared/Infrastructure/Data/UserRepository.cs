using Dapper;
using LibrarySystem.Shared.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly SqliteConnectionFactory _connectionFactory;

        public UserRepository(SqliteConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<(string Name, int BorrowCount)>> GetTopUsers(DateTime start, DateTime end)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "select u.Name, COUNT(*) as LendingCount " +
                            "FROM Lendings l " +
                            "INNER JOIN Users u ON u.Id = l.UserId " +
                            "WHERE l.BorrowDate BETWEEN @start AND @end " +
                            "GROUP BY l.UserId";

            var users = await connection.QueryAsync<(string, int)>(
                   sql,
                   new { start, end } // passing parameters to Dapper
               );
            return users;
        }

        public async Task<IEnumerable<string>> GetUserBorrowedBooks(int id, DateTime start, DateTime end)
        {
            using var connection = _connectionFactory.CreateConnection();

            string sql = "select b.title " +
                         "FROM Lendings l " +
                         "INNER JOIN Books b ON l.BookID = b.Id " +
                         "WHERE l.UserID = @id " +
                         "and BorrowDate between @start and @end";

            var booktitles = await connection.QueryAsync<string>(
                   sql,
                   new { id, start, end } // passing parameters to Dapper
               );
            return booktitles;
        }



        }
}
