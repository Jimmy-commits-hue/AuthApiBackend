using AuthApi.Database;
using AuthApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Repository
{

    public class CustomerNumberRepo : ICustomNumberRepo
    {

        private readonly AuthApiDbContext _db;

        public CustomerNumberRepo(AuthApiDbContext _db) => this._db = _db; 

        public async Task<string?> GetLastCustomNumber()
        {

            string? lastNumber = await _db.User
                .Where(u => u.isVerified && u.customNumber.StartsWith(DateTime.Now.Year.ToString()))
                .OrderByDescending(u => u.customNumber)
                .Select(u => u.customNumber)
                .FirstOrDefaultAsync();

            return lastNumber;

        }

    }

}