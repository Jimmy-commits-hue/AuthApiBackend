using Web.Database;
using Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Web.Repository
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