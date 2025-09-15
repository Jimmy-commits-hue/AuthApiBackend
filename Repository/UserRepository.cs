using Web.Database;
using Web.Interfaces;
using Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Web.Repository
{

    public class UserRepository : IUserRepository
    {

        private readonly AuthApiDbContext context;

        public UserRepository(AuthApiDbContext context) => this.context = context;

        public async Task CreateAsync(User user)
        {

            context.User.Add(user);
            await context.SaveChangesAsync();
         
        }

        public async Task<User?> GetAsync(string customerNumber)
        {

            return await context.User.Where(u => u.customNumber == customerNumber).FirstOrDefaultAsync();

        }

        public async Task<string> GetUserByCustomNumber(string customNumber)
        {

            return await context.User.Where(u => u.customNumber == customNumber).Select(u => u.FirstName).FirstAsync();

        }

        public async Task<User?> GetUserByIdNumber(string idNumber)
        {

            return await context.User.Where(u => u.IdNumber.CompareTo(idNumber) == 0).FirstOrDefaultAsync();

        }

        public async Task<User?> GetUserById(Guid userId)
        {

            return await context.User.FindAsync(userId);

        }

        public async Task<Guid?> GetByEmailAsync(string email)
        {

            return await context.User.Where(e => e.Email.CompareTo(email) == 0).Select(u => u.Id).FirstOrDefaultAsync();

        }

        public async Task UpdateAsync(User user)
        {

            context.User.Update(user);
            await context.SaveChangesAsync();

        }

        public async Task DeleteAsync(User user)
        {

            context.User.Remove(user);
            await context.SaveChangesAsync();

        }

    }

}