using LibrarySystem.GrpcService;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService.BookServiceClient _grpcClient;

        public BooksController(BookService.BookServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var grpcRequest = new GetAllBooksRequest();

            var grpcResponse = await _grpcClient.GetAllBooksAsync(grpcRequest);

            // Map gRPC response to API response as needed
            var books = grpcResponse.Books;

            return Ok(books);
        }

        [HttpGet("most-borrowed")]
        public async Task<IActionResult> GetMostBorrowedBooks()
        {
            var grpcRequest = new GetMostBorrowedBooksRequest();            
            var grpcResponse = await _grpcClient.GetMostBorrowedBooksAsync(grpcRequest);

            // Map gRPC response to API response as needed
            var books = grpcResponse.Books;

            return Ok(books);
        }
    }
}
