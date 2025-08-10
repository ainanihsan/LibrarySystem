using LibrarySystem.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Application.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Books>> GetAllAsync();
        Task<IEnumerable<(string, int)>> GetMostBorrowedBooks();
        Task<Books?> GetByIdAsync(int id);
    }
}
