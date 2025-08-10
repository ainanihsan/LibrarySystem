namespace LibrarySystem.Shared.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<(string Name, int BorrowCount)>> GetTopUsers(DateTime start, DateTime end);
    }
}