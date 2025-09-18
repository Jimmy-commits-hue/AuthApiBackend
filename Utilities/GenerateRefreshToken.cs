using System.Security.Cryptography;

namespace AuthApi.Utilities
{

    public static class GenerateRefreshToken
    {

        public static string Generate()
        {

            byte[] bytes = new byte[32];
            var generate = RandomNumberGenerator.Create();
            generate.GetBytes(bytes);

            return Convert.ToBase64String(bytes);

        }

    }

}