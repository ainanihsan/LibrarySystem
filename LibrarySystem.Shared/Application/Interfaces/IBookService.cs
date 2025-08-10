
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Books>> GetAllBooksAsync();
        Task<IEnumerable<(string, int)>> GetMostBorrowedBooksAsync();
        Task<Books?> GetBookByIdAsync(int id);
    }
}
