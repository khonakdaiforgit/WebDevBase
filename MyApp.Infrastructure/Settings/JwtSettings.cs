

namespace MyApp.Infrastructure.Settings
{
    public class JwtSettings
    {
        public string Secret { get; set; } = "your-super-secret-jwt-key-min-32-chars";
        public string Issuer { get; set; } = "RestaurantApi";
        public string Audience { get; set; } = "RestaurantClients";
        public int ExpiryMinutes { get; set; } = 60;
    }
}
