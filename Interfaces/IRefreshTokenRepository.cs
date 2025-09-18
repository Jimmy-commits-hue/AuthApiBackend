using AuthApi.Models;

namespace AuthApi.Interfaces
{

    public interface IRefreshTokenRepository
    {

        Task CreateRefreshTokenAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        Task InvalidateRefreshTokenAsync(RefreshToken refreshToken);

    }

}
