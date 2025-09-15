using Microsoft.IdentityModel.Tokens;
using Web.Models;

namespace Web.Security
{
    public class GenerateJwtToken
    {

        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenerateJwtToken(IConfiguration _config, IHttpContextAccessor httpContextAccessor)
        {

            this._config = _config;
            _httpContextAccessor = httpContextAccessor;

        }

        public string GenerateToken(User user)
        {

            var key = new SymmetricSecurityKey(Convert.FromBase64String(Environment.GetEnvironmentVariable("JWT_KEY")!));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {

                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.FirstName),
                new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.customNumber),
                new System.Security.Claims.Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(

                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: signingCredentials

            );

            var tokenString = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
            
            return tokenString;
        }

    }

}