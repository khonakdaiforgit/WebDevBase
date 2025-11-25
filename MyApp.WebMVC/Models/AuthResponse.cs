namespace MyApp.WebMVC.Models
{
    public record AuthResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);

}
