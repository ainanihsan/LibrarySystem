
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
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

        public async Task<Books?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }
    }
}
