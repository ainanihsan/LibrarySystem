
using Grpc.Core;
using LibrarySystem.GrpcService;
using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Application.Services;
using UserService = LibrarySystem.GrpcService.UserService;

    public class UserGrpcService : UserService.UserServiceBase
    {
        private readonly IUserService _userService;

        public UserGrpcService(IUserService userService)
        {
            _userService = userService;
        }

        public override async Task<GetTopUsersReply> GetTopUsers(GetTopUsersRequest request, ServerCallContext context)
        {
            var users = await _userService.GetTopUsersAsync(request.StartDate.ToDateTime(), request.EndDate.ToDateTime());

            var reply = new GetTopUsersReply();
            foreach (var user in users)
            {
                reply.User.Add(new User
                {
                    Name = user.Item1,
                    Count = user.Item2
                });
            }

            return reply;
        }
    }
