using AuthApi.Models;

namespace AuthApi.Interfaces
{

    public interface IUserRepository
    {

        Task CreateAsync(User user, CancellationToken cancellationToken);

        Task<User?> GetAsync(string customNumber, CancellationToken cancellationToken);

        Task<User?> GetUserByIdNumber(string idNumber, CancellationToken cancellationToken);

        Task<string> GetUserByCustomNumber(string customNumber);

        Task UpdateAsync(User user);

        Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken);

        Task DeleteAsync(User user);

    }

}
