using Web.Interfaces;
using Web.Utilities;

namespace Web.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {

        private readonly IRefreshTokenRepository _refreshTokenRepository;


        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {

            _refreshTokenRepository = refreshTokenRepository;

        }

        public async Task CreateAsync(string token, Guid userId)
        {
           
            await _refreshTokenRepository.CreateRefreshTokenAsync(new Models.RefreshToken
            {

                userId = userId,
                token = HashHelper.HashId(token),
                isActive = true,
                expires = DateTime.UtcNow.AddHours(24),

            });

        }

        public async Task<Models.RefreshToken?> GetTokenAsync(string token)
        {

            return await _refreshTokenRepository.GetRefreshTokenAsync(token);

        }
        public async Task InvalidateTokenAsync(Models.RefreshToken refreshToken)
        {

            refreshToken.isActive = false;
            await _refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken);

        }

    }

}