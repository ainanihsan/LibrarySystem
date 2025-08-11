
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
        private Mock<ILendingRepository> _lendingRepositoryMock;
        private IBookServices _bookService;

        [TestInitialize]
        public void Setup()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _lendingRepositoryMock = new Mock<ILendingRepository>();
            _bookService = new BookServices(_bookRepositoryMock.Object,_lendingRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetAllBooksAsync_ReturnsAllBooks()
        {
            // Arrange
            var books = new List<Books>
            {
                new Books { Id = 1, Title = "Book 1", Author = "Author A" },
                new Books { Id = 2, Title = "Book 2", Author = "Author B" }
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


        [TestMethod]
        public async Task GetMostBorrowedBooksAsync_ReturnsBooksInDescendingOrder()
        {
            // Arrange
            var mostBorrowedBooks = new List<(string Title, int BorrowCount)>
            {
                ("Clean Code", 5),
                ("The Hobbit", 3),
                ("1984", 2)
            };

            _bookRepositoryMock
                .Setup(repo => repo.GetMostBorrowedBooks())
                .ReturnsAsync(mostBorrowedBooks);

            // Act
            var result = await _bookService.GetMostBorrowedBooksAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.SequenceEqual(result.OrderByDescending(b => b.Item2)),
                "Books are not ordered by BorrowCount descending");

            Assert.IsTrue(result.Any(b => b.Item1 == "Clean Code"));
        }

        [TestMethod]
        public async Task GetBookStats_ReturnsCopiesTotalAndBorrowedCount()
        {
            // Arrange
            int bookId = 1;
            int copiesTotal = 5;
            int borrowedCount = 2;

            _bookRepositoryMock
                .Setup(repo => repo.GetBookStats(bookId))
                .ReturnsAsync((copiesTotal, borrowedCount));

            // Act
            var result = await _bookService.GetBookStats(bookId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(copiesTotal, result.Item1);
            Assert.AreEqual(borrowedCount, result.Item2);
        }

        [TestMethod]
        public async Task CalculateReadingRateAsync_ReturnsCorrectReadingRate()
        {
            // Arrange
            int bookId = 1;

            var borrowRecords = new List<LendingRecord>
            {
                new LendingRecord { BorrowDate = new DateTime(2024, 7, 1), ReturnDate = new DateTime(2024, 7, 6), Pages = 300 },  // 5 days reading
                new LendingRecord { BorrowDate = new DateTime(2024, 7, 10), ReturnDate = new DateTime(2024, 7, 15), Pages = 300 }, // 5 days reading
                new LendingRecord { BorrowDate = new DateTime(2024, 7, 20), ReturnDate = DateTime.MinValue, Pages = 300 } // ignored because ReturnDate is set to a default value
            };

            _lendingRepositoryMock
                .Setup(repo => repo.GetBorrowRecordsForBook(bookId))
                .ReturnsAsync(borrowRecords);

            // Act
            var readingRate = await _bookService.GetReadingEstimate(bookId);

            // Assert
            // Total days = 5 + 5 = 10, pages = 300 => 300 / 10 = 30 pages/day
            Assert.AreEqual(60, readingRate, 0.001);
        }


    }
}
