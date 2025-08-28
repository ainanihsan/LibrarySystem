using Google.Protobuf.WellKnownTypes;
using LibrarySystem.GrpcClient;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService.UserServiceClient _grpcClient;
        private ILogger<UsersController> _logger;

        public UsersController(UserService.UserServiceClient grpcClient, ILogger<UsersController> logger)
        {
            _grpcClient = grpcClient;
            _logger = logger;
        }

        [HttpGet("top-users")]
        public async Task<IActionResult> GetTopUsers(DateTime startDate, DateTime endDate)
        {
            var grpcRequest = new GetTopUsersRequest
            {
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(endDate.ToUniversalTime())
            };
            try
            {
                var grpcResponse = await _grpcClient.GetTopUsersAsync(grpcRequest);
                var users = grpcResponse.User;
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching top users.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet("user-books")]
        public async Task<IActionResult> GetUserBorrowedBooks(int id, DateTime startDate, DateTime endDate)
        {
            var grpcRequest = new GetUserBorrowedBooksRequest
            {
                Id = id,
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(endDate.ToUniversalTime())
            };
            try
            {
                var grpcResponse = await _grpcClient.GetUserBorrowedBooksAsync(grpcRequest);
                var books = grpcResponse.Book;
                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user borrowed books.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
