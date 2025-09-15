using Web.Models;

namespace Web.Interfaces
{

    public interface IUserRepository
    {

        Task CreateAsync(User user);

        Task<User?> GetAsync(string customNumber);

        Task<User?> GetUserByIdNumber(string idNumber);

        Task<string> GetUserByCustomNumber(string customNumber);

        Task UpdateAsync(User user);

        Task<User?> GetUserById(Guid userId);

        Task DeleteAsync(User user);

    }

}
