using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MyApp.WebMVC.Helper
{
    public static class TokenHelper
    {
        public static string GetUserIdFromToken(string token)
        {
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring("Bearer ".Length);
            }

            if (string.IsNullOrEmpty(token) || token.Split('.').Length != 3)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) ??
                                  jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");
                if (userIdClaim == null)
                {
                    return null;
                }

                return userIdClaim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JWT: {ex.Message}");
                return null;
            }
        }
    }
}
