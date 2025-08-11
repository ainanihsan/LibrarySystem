
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Application.Services
{
    public class BookServices : IBookServices
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILendingRepository _lendingRepository;

        public BookServices(IBookRepository bookRepository, ILendingRepository lendingRepository)
        {
            _bookRepository = bookRepository;
            _lendingRepository = lendingRepository;
        }

        public async Task<IEnumerable<Books>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<IEnumerable<(string,int)>> GetMostBorrowedBooksAsync()
        {
            return await _bookRepository.GetMostBorrowedBooks();
        }

        public async Task<IEnumerable<string>> GetOtherBooksBorrowedBySamePeople(int bookId)
        {
            return await _bookRepository.GetOtherBooksBorrowedBySamePeople(bookId);
        }

        public async Task<(int, int)> GetBookStats(int id)
        {
            return await _bookRepository.GetBookStats(id);
        }

        public async Task<double> GetReadingEstimate(int bookId)
        {
            var lendingRecords = await _lendingRepository.GetBorrowRecordsForBook(bookId);

            if (!lendingRecords.Any())
                throw new InvalidOperationException("No borrow records found for this book.");

            double totalRate = 0;
            int count = 0;

            foreach (var record in lendingRecords)
            {
                if (record.ReturnDate.ToString() == null || record.ReturnDate <= record.BorrowDate)
                    continue; // skip invalid records

                var days = (record.ReturnDate - record.BorrowDate).TotalDays;
                if (days <= 0)
                    continue;

                double rate = record.Pages / days;
                totalRate += rate;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException("No valid borrow records to calculate reading rate.");

            return totalRate / count; // average rate
        }

        public async Task<Books?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }
    }
}
