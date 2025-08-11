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

        public UsersController(UserService.UserServiceClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        [HttpGet("top-users")]
        public async Task<IActionResult> GetTopUsers(DateTime startDate, DateTime endDate)
        {
            var grpcRequest = new GetTopUsersRequest
            {
                StartDate = Timestamp.FromDateTime(startDate.ToUniversalTime()),
                EndDate = Timestamp.FromDateTime(endDate.ToUniversalTime())
            };
            var grpcResponse = await _grpcClient.GetTopUsersAsync(grpcRequest);
            var users = grpcResponse.User;
            return Ok(users);
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
            var grpcResponse = await _grpcClient.GetUserBorrowedBooksAsync(grpcRequest);
            var books = grpcResponse.Book;
            return Ok(books);
        }

    }
}
