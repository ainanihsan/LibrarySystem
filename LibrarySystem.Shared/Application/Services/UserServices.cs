using LibrarySystem.Shared.Application.Interfaces;
using LibrarySystem.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<(string,int)>> GetTopUsersAsync(DateTime start, DateTime end)
        {
            return await _userRepository.GetTopUsers(start,end);
        }

        public async Task<IEnumerable<string>> GetUserBorrowedBooks(int id, DateTime start, DateTime end)

        {
            return await _userRepository.GetUserBorrowedBooks(id,start,end);
        }

    }
}
