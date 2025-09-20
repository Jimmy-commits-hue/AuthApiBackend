using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Utilities
{

    public static class HashHelper
    {

        public static string HashId(string id)
        {

            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY") + id);
            var computeHash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(computeHash);

        }

        public static string HashPassword(string password)
        {

            return new PasswordHasher<string>().HashPassword(Environment.GetEnvironmentVariable("SECRET_KEY")!
                                                             , password);

        }

        public static PasswordVerificationResult VerifyHashPassword(string hashedPassword, string password)
        {

            return new PasswordHasher<string>().VerifyHashedPassword(Environment.GetEnvironmentVariable("SECRET_KEY")!
                                                                     , hashedPassword, password);

        }

    }

}
