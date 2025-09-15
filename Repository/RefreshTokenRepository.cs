using Web.Database;
using Web.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Web.Repository
{

    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        
        private readonly AuthApiDbContext db;

        public RefreshTokenRepository(AuthApiDbContext db)
        {

            this.db = db;

        }

        public async Task CreateRefreshTokenAsync(Models.RefreshToken token)
        {

            db.RefreshToken.Add(token);
            await db.SaveChangesAsync();

        }

        public async Task<Models.RefreshToken?> GetRefreshTokenAsync(string token)
        {

            return await db.RefreshToken.FirstOrDefaultAsync(t => t.token == token);

        }

        public async Task InvalidateRefreshTokenAsync(Models.RefreshToken token)
        {

            db.RefreshToken.Update(token);
            await db.SaveChangesAsync();

        }

    }

}