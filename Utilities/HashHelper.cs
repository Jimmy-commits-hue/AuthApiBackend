using System.Security.Cryptography;
using System.Text;

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

    }

}
