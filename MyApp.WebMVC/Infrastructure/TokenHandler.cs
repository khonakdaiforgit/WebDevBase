using MyApp.Application;
using MyApp.WebMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyApp.WebMVC.Infrastructure
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return await base.SendAsync(request, cancellationToken);

            var accessToken = context.Request.Cookies["access_token"];
            var refreshToken = context.Request.Cookies["refresh_token"];

            // اگر توکن نداشتیم → مستقیم برو
            if (string.IsNullOrEmpty(accessToken))
                return await base.SendAsync(request, cancellationToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await base.SendAsync(request, cancellationToken);

            //// اگر 401 بود → سعی کن رفرش کنی
            //if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized
            //    && !string.IsNullOrEmpty(refreshToken))
            //{
            //    var newTokens = await TryRefreshTokenAsync(refreshToken, context);
            //    if (newTokens != null)
            //    {
            //        // دوباره درخواست اصلی رو با توکن جدید بفرست
            //        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);
            //        response = await base.SendAsync(request, cancellationToken);
            //    }
            //}

            return response;
        }

        private async Task<AuthResponse?> TryRefreshTokenAsync(string refreshToken, HttpContext context)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(AppUrl.GetApiUrl());

            var content = new StringContent(
                JsonSerializer.Serialize(new { refreshToken }),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/auth/refresh", content);

            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (result == null) return null;

            // بروزرسانی کوکی‌ها
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiresAt
            };

            context.Response.Cookies.Append("access_token", result.AccessToken, cookieOptions);
            context.Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return result;
        }
    }

}