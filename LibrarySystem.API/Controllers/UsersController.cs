using Google.Protobuf.WellKnownTypes;
using LibrarySystem.GrpcService;
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

            // Map gRPC response to API response as needed
            var users = grpcResponse.User;

            return Ok(users);
        }


    }
}
