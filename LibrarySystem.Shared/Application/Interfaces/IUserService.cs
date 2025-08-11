using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Application.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<(string, int)>> GetTopUsersAsync(DateTime start, DateTime end);
        Task<IEnumerable<string>> GetUserBorrowedBooks(int id, DateTime start, DateTime end);
    }
}
