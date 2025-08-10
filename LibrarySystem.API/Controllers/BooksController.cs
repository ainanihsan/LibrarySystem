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

        [HttpGet("book-stats/{bookId}")]
        public async Task<IActionResult> GetBookStats(int bookId)
        {
            var grpcRequest = new GetBookStatsRequest { Id = bookId };
            var grpcResponse = await _grpcClient.GetBookStatsAsync(grpcRequest);

            // Calculate available copies
            int available = grpcResponse.CopiesTotal - grpcResponse.Borrowed;

            var response = new
            {
                CopiesTotal = grpcResponse.CopiesTotal,
                BorrowedCount = grpcResponse.Borrowed,
                AvailableCopies = available < 0 ? 0 : available
            };

            return Ok(response);
        }



    }
}
