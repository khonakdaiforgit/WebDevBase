namespace MyApp.Infrastructure.Services.Auth.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = "your-super-secret-key-with-at-least-32-chars";
        public string Issuer { get; set; } = "MyApp";
        public string Audience { get; set; } = "MyAppClient";
        public int AccessTokenExpiryMinutes { get; set; } = 60;
        public int RefreshTokenExpiryDays { get; set; } = 7;
    }
}
