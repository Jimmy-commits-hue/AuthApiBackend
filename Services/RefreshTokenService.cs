using AuthApi.Interfaces;
using AuthApi.Utilities;

namespace AuthApi.Services
{
    /// <summary>
    /// Provides functionality for managing refresh tokens, including creation, retrieval, and invalidation.
    /// </summary>
    /// <remarks>This service is responsible for handling operations related to refresh tokens, such as
    /// creating new tokens, retrieving existing tokens, and marking tokens as invalid. It interacts with an underlying
    /// repository to persist and manage refresh token data.</remarks>
    public class RefreshTokenService : IRefreshTokenService
    {

        private readonly IRefreshTokenRepository _refreshTokenRepository;


        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {

            _refreshTokenRepository = refreshTokenRepository;

        }
        /// <summary>
        /// It is used to create a new refresh token for a user.
        /// </summary>
        /// <param name="token">random generated number</param>
        /// <param name="userId">User primary key to associate with the token</param>
        /// <remarks>
        /// <para>We hash the refresh token being stored in the database and add <see cref="DateTime"/> track its 
        /// expiration and mark it as <see langword="true"/> to show its active</para>
        /// </remarks>

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

        /// <summary>
        /// Retrieves a refresh token based on the provided token string.
        /// </summary>
        /// <remarks>This method queries the underlying repository to retrieve the refresh token
        /// associated with the specified token string. Ensure that the provided token is valid.</remarks>
        /// <param name="token">The token string used to identify the refresh token. Cannot be null or empty.</param>
        /// <returns>A <see cref="Models.RefreshToken"/> object if a matching token is found; otherwise, 
        /// <see langword="null"/>.</returns>
        public async Task<Models.RefreshToken?> GetTokenAsync(string token)
        {

            return await _refreshTokenRepository.GetRefreshTokenAsync(token);

        }

        /// <summary>
        /// Marks the specified refresh token as inactive and updates its state in the repository.
        /// </summary>
        /// <param name="refreshToken">The refresh token to invalidate. Must not be null 
        /// and must represent an active token.</param>
        public async Task InvalidateTokenAsync(Models.RefreshToken refreshToken)
        {

            refreshToken.isActive = false;
            await _refreshTokenRepository.InvalidateRefreshTokenAsync(refreshToken);

        }

    }

}