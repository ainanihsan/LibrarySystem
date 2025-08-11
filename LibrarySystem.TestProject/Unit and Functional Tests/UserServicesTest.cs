using Moq;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Application.Services;
using LibrarySystem.Shared.Domain;

namespace LibrarySystem.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private IUserServices _userService;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserServices(_userRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetTopUsersAsync_ReturnsUsersWithinDateRange()
        {
            // Arrange
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 1, 31);

            var topUsers = new List<(string Name, int BorrowCount)>
            {
                ("Alice", 5),
                ("Bob", 3),
                ("Charlie", 2)
            };

            _userRepositoryMock
                .Setup(repo => repo.GetTopUsers(startDate, endDate))
                .ReturnsAsync(topUsers);

            // Act
            var result = await _userService.GetTopUsersAsync(startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Alice", result.First().Item1);
            Assert.AreEqual(5, result.First().Item2);
        }
        [TestMethod]
        public async Task GetUserBorrowedBooksAsync_ReturnsBooksForUserWithinDateRange()
        {
            // Arrange
            var userId = 1;
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 1, 31);

            var borrowedBooks = new List<string>
            {
                "Book A",
                "Book B",
                "Book C"
            };

            _userRepositoryMock
                .Setup(repo => repo.GetUserBorrowedBooks(userId, startDate, endDate))
                .ReturnsAsync(borrowedBooks);

            // Act
            var result = await _userService.GetUserBorrowedBooks(userId, startDate, endDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.IsTrue(result.Contains("Book A"));
            Assert.IsTrue(result.Contains("Book B"));
            Assert.IsTrue(result.Contains("Book C"));
        }



    }
}
