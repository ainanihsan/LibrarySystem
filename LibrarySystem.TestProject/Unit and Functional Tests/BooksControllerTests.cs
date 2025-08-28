
using Grpc.Core;
using LibrarySystem.API.Controllers;
using LibrarySystem.GrpcClient;
using LibrarySystem.Shared.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LibrarySystem.TestProject
{
    [TestClass]
    public class BooksControllerTests
    {
        private BooksController booksController;
        private Mock<BookService.BookServiceClient> grpcClientMock;
        private ILogger<BooksController> logger;

        [TestInitialize]
        public void Setup()
        {
            grpcClientMock = new Mock<BookService.BookServiceClient>();
            logger = new NullLogger<BooksController>();

            grpcClientMock
                .Setup(c => c.GetAllBooksAsync(
                    It.IsAny<GetAllBooksRequest>(),
                    null,
                    null,
                    default))
                .Returns(new AsyncUnaryCall<GetAllBooksReply>(
                    Task.FromResult(new GetAllBooksReply
                    {
                        Books = { new GetBookReply { Id = 1, Title = "Test Book", Author = "Author" } }
                    }),
                    Task.FromResult(new Metadata()),
                    () => Status.DefaultSuccess,
                    () => new Metadata(),
                    () => { }
            ));

            booksController = new BooksController(grpcClientMock.Object, logger);
        }

        [TestMethod]
        public async Task GetAllBooks_ReturnsOkResult()
        {
            var result = await booksController.GetAllBooks() as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Value, typeof(Google.Protobuf.Collections.RepeatedField<GetBookReply>));
        }
    }
}