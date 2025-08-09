
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
    }
}
