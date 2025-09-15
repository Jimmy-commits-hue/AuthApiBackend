namespace Web.Interfaces
{

    public interface IRefreshTokenService
    {

        Task CreateAsync(string refreshToken, Guid userId);

        Task<Models.RefreshToken?> GetTokenAsync(string token);

        Task InvalidateTokenAsync(Models.RefreshToken token);

    }

}
