
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Application.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Books>> GetAllBooksAsync();
        Task<IEnumerable<(string, int)>> GetMostBorrowedBooksAsync();
        Task<(int, int)> GetBookStats(int id);
        Task<Books?> GetBookByIdAsync(int id);
        Task<IEnumerable<string>> GetOtherBooksBorrowedBySamePeople(int bookId);
        Task<double> GetReadingEstimate(int bookId);
    }
}
