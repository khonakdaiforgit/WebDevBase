
using System.Security.Cryptography;
using System.Text;

namespace MyApp.Application.Helper
{
    public static class HashHelper
    {
        public static string HashIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return "Unknown"; using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(ip));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
