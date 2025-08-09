
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

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }
    }
}
