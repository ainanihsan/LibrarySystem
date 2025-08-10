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
        private IUserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
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
    }
}
