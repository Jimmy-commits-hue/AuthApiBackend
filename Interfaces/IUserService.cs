using Web.DTOs;
using Web.Models;

namespace Web.Interfaces
{

    public interface IUserService
    {

        Task<Guid> CreateUserAsync(RegisterDto user);

        Task<User?> GetUserAsync(LoginDtos login);

        Task<User> GetUserById(Guid userId);

        Task<string> GetUserNameByCustomNumber(string customNumber);

        Task UpdateUserNumberAsync(string customNumber, User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(DeleteUserDtos deleteUser);

    }

}
