using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MyApp.Application.DTOs;
using MyApp.Application.Helper;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MyApp.WebMVC.Handlers
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly HttpClient _client;

        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        public JwtTokenHandler(IHttpContextAccessor contextAccessor, IHttpClientFactory httpClientFactory)
        {
            _contextAccessor = contextAccessor;
            _client = httpClientFactory.CreateClient("publicApi");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _contextAccessor.HttpContext;
            var token = httpContext.Items.TryGetValue("JwtToken", out var storedToken)
                        ? storedToken?.ToString()
                        : httpContext.User.Claims.FirstOrDefault(c => c.Type == "JwtToken")?.Value;

            var refreshToken = httpContext?.Request.Cookies["RefreshToken"];
            bool needRefresh = false;

            if (string.IsNullOrEmpty(refreshToken))
            {
                httpContext!.Response.Redirect("/Account/Login");
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }

            if (!string.IsNullOrEmpty(token))
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                if (jwt.ValidTo <= DateTime.UtcNow)
                {
                    needRefresh = true;
                }
            }
            else
            {
                needRefresh = true; // اگر توکن وجود نداشته باشد، نیاز به رفرش است
            }

            if (needRefresh)
            {
                var lockKey = "refresh_lock:" + refreshToken;
                var semaphore = _locks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));

                await semaphore.WaitAsync(cancellationToken);

                try
                {

                    var response = await _client.PostAsJsonAsync("user/refresh", new TokenRequestDto
                    {
                        AccessToken = token,
                        RefreshToken = refreshToken
                    }, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        //httpContext.Response.Cookies.Delete("RefreshToken");
                        //httpContext.Response.Redirect("/Account/Login");
                        await httpContext.SignOutAsync();
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }

                    var data = await response.Content.ReadFromJsonAsync<TokenRequestDto>(cancellationToken: cancellationToken);

                    if (data == null || string.IsNullOrEmpty(data.AccessToken) || string.IsNullOrEmpty(data.RefreshToken))
                    {
                        //httpContext.Response.Cookies.Delete("RefreshToken");
                        //httpContext.Response.Redirect("/Account/Login");
                        await httpContext.SignOutAsync();
                        return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                    }

                    var newJwt = new JwtSecurityTokenHandler().ReadJwtToken(data.AccessToken);

                    // به‌روزرسانی RefreshToken
                    httpContext.Response.Cookies.Append("RefreshToken", data.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // تنظیم به true برای امنیت
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(AppSettingsHelper.refreshTokenExpireAfterMints)
                    });

                    token = data.AccessToken;


                    // تنظیم Claims
                    var claims = newJwt.Claims.ToList();
                    claims.Add(new Claim("JwtToken", token));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(AppSettingsHelper.refreshTokenExpireAfterMints), // همگام‌سازی با توکن JWT
                        IssuedUtc = DateTime.UtcNow
                    };

                    // انجام SignIn روی httpContext
                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);


                    httpContext.Items["JwtToken"] = token;

                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    return await base.SendAsync(request, cancellationToken);
                }
                catch (Exception ex)
                {
                    // لاگ کردن خطا
                    // logger.LogError(ex, "Error refreshing token");
                    //httpContext.Response.Cookies.Delete("RefreshToken");
                    //httpContext.Response.Redirect("/Account/Login");
                    await httpContext.SignOutAsync();
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
                finally
                {
                    semaphore.Release();
                    _locks.TryRemove(lockKey, out _);
                }
            }

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
