using Microsoft.IdentityModel.Tokens;
using MyApp.Application;
using MyApp.Application.Abstractions.Users.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApp.WebMVC.Middlewares;

public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtRefreshMiddleware> _logger;

    public JwtRefreshMiddleware(
        RequestDelegate next, 
        IConfiguration configuration, 
        ILogger<JwtRefreshMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // اول درخواست رو اجرا کن (Authentication هم اجرا میشه)
        await _next(context);

        // فقط اگر 401 بود و کوکی refresh_token داشت
        if (context.Response.StatusCode != 401 ||
            !context.Request.Cookies.TryGetValue("refresh_token", out var refreshToken) ||
            string.IsNullOrEmpty(refreshToken))
        {
            return;
        }

        _logger.LogInformation("Access token expired. Attempting automatic refresh...");

        try
        {
            var newTokens = await RefreshTokenAsync(refreshToken, context);
            if (newTokens == null)
            {
                _logger.LogWarning("Refresh failed → Redirecting to login");
                RedirectToLogin(context);
                return;
            }

            // دستی کاربر رو authenticate کن (مهم!)
            var principal = ValidateToken(newTokens.AccessToken);
            if (principal == null)
            {
                _logger.LogError("New token is invalid after refresh!");
                RedirectToLogin(context);
                return;
            }

            // ست کردن کاربر برای این درخواست
            context.User = principal;

            // پاک کردن 401 و ادامه درخواست با توکن جدید
            context.Response.StatusCode = 200;
            context.Response.Headers.Remove("WWW-Authenticate");

            // حالا دوباره کنترلر رو صدا بزن (این بار با کاربر لاگین شده)
            await _next(context);

            _logger.LogInformation("Token refreshed successfully for {Email}", principal.FindFirst(ClaimTypes.Email)?.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in JwtRefreshMiddleware");
            RedirectToLogin(context);
        }
    }

    private async Task<AuthResult?> RefreshTokenAsync(string refreshToken, HttpContext context)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri(AppUrl.GetApiUrl());

        //var apiUrl = _configuration["ApiSettings:BaseUrl"] ?? context.Request.Scheme + "://" + context.Request.Host;

        var response = await client.PostAsync($"api/auth/refresh",
            new StringContent($"{{\"refreshToken\":\"{refreshToken}\"}}", Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadFromJsonAsync<AuthResult>();
        if (result?.AccessToken == null) return null;

        // بروزرسانی کوکی‌ها
        var accessOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = result.ExpiresAt
        };

        var refreshOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        };

        context.Response.Cookies.Append("access_token", result.AccessToken, accessOptions);
        context.Response.Cookies.Append("refresh_token", result.RefreshToken, refreshOptions);

        context.Items["__LatestAccessToken"] = result.AccessToken;

        return result;
    }

    private static void RedirectToLogin(HttpContext context)
    {
        var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
        context.Response.Redirect($"/Account/Login?returnUrl={returnUrl}");
    }
    private ClaimsPrincipal? ValidateToken(string jwtToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
    private ClaimsPrincipal? ValidateAndGetPrincipal(string jwtToken)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}