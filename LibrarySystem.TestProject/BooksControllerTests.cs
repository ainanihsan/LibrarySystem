using LibrarySystem.API;
using LibrarySystem.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace LibrarySystem.TestProject
{
    [TestClass]
    public sealed class BooksControllerTests
    {
        static BooksController booksController;


        [TestInitialize]
        public void Setup()
        {
            booksController = new BooksController(NullLogger<BooksController>.Instance);
        }

        [TestMethod]
        public async Task GetAllBooks()
        {
            var result = await booksController.GetAllBooks() as OkObjectResult;
            Assert.IsNotNull(result);
        }
    }
}
