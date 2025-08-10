using Grpc.Core;
using LibrarySystem.API;
using LibrarySystem.API.Controllers;
using LibrarySystem.GrpcService;
using LibrarySystem.Shared.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LibrarySystem.TestProject
{
    [TestClass]
    public class BooksControllerTests
    {
        private BooksController booksController;
        private Mock<BookService.BookServiceClient> grpcClientMock;

        [TestInitialize]
        public void Setup()
        {
            grpcClientMock = new Mock<BookService.BookServiceClient>();

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


            booksController = new BooksController(grpcClientMock.Object);
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