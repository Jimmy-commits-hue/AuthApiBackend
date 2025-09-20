using AuthApi.DTOs;
using AuthApi.Models;

namespace AuthApi.Interfaces
{

    public interface IUserService
    {

        Task<Guid> CreateUserAsync(RegisterDto user, CancellationToken cancellationToken);

        Task<User> GetUserAsync(LoginDtos login, CancellationToken cancellationToken);

        Task<User> GetUserById(Guid userId, CancellationToken cancellationToken);

        Task<string> GetUserNameByCustomNumber(string customNumber);

        Task UpdateUserNumberAsync(string customNumber, User user);

        Task UpdateUserAsync(User user, CancellationToken cancellationToken);

        Task IsLoginEmailSent(User user);

        Task DeleteUserAsync(DeleteUserDtos deleteUser, CancellationToken cancellationToken);

    }

}
