using LibrarySystem.GrpcClient;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly BookService.BookServiceClient _grpcClient;
        private ILogger _logger;

        public BooksController(BookService.BookServiceClient grpcClient , ILogger logger)
        {
            _grpcClient = grpcClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var grpcRequest = new GetAllBooksRequest();
            try
            {
                var grpcResponse = await _grpcClient.GetAllBooksAsync(grpcRequest);
                var books = grpcResponse.Books;
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all books.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("most-borrowed")]
        public async Task<IActionResult> GetMostBorrowedBooks()
        {
            var grpcRequest = new GetMostBorrowedBooksRequest();
            try
            {
                var grpcResponse = await _grpcClient.GetMostBorrowedBooksAsync(grpcRequest);
                var books = grpcResponse.Books;
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching most borrowed books.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("book-stats/{bookId}")]
        public async Task<IActionResult> GetBookStats(int bookId)
        {
            var grpcRequest = new GetBookStatsRequest { Id = bookId };
            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching book stats.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("book-same-people/{bookId}")]
        public async Task<IActionResult> GetOtherBooksBorrowedBySamePeople(int bookId)
        {
            var grpcRequest = new GetOtherBooksBorrowedBySamePeopleRequest { BookId = bookId };
            try
            {
                var grpcResponse = await _grpcClient.GetOtherBooksBorrowedBySamePeopleAsync(grpcRequest);
                var books = grpcResponse.Book;
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching other books borrowed by same people.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("reading-estimate/{bookId}")]
        public async Task<IActionResult> GetReadingEstimate(int bookId)
        {
            var grpcRequest = new GetReadingEstimateRequest { BookId = bookId };
            try
            {
                var grpcResponse = await _grpcClient.GetReadingEstimateAsync(grpcRequest);
                var rate = grpcResponse.AverageRate;
                return Ok(rate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching reading estimate.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
