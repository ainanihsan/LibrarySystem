using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Shared.Application.Interfaces
{
    public interface ILendingRepository
    {
        Task<IEnumerable<LendingRecord>> GetBorrowRecordsForBook(int bookId);
    }
}