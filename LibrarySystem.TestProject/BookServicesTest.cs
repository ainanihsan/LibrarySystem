
using Moq;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Application.Services;
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Tests
{
    [TestClass]
    public class BookServiceTests
    {
        private Mock<IBookRepository> _bookRepositoryMock;
        private IBookService _bookService;

        [TestInitialize]
        public void Setup()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _bookService = new BookService(_bookRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetAllBooksAsync_ReturnsAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author A" },
                new Book { Id = 2, Title = "Book 2", Author = "Author B" }
            };

            _bookRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(books);

            // Act
            var result = await _bookService.GetAllBooksAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }
    }
}
