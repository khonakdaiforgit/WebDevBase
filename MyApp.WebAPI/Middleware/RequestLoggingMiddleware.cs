using MyApp.Application.Abstractions.Logging;
using MyApp.Application.Abstractions.Logging.Dtos;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MyApp.WebAPI.Middleware
{

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly string[] _excludedExtensions = { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".woff", ".woff2", ".ttf", ".ico" };
        private readonly string[] _excludedPaths = { "/swagger", "/favicon.ico", "/api/logs" };
        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogService logService)
        {
            var path = context.Request.Path.Value?.ToLower();
            if (string.IsNullOrEmpty(path) ||
                _excludedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) ||
                _excludedPaths.Any(excludedPath => path.StartsWith(excludedPath, StringComparison.OrdinalIgnoreCase)))
            {
                // درخواست مستثنی است، فقط ادامه بده
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            var response = context.Response;

            var logDto = new LogEntryDto(
                Id: Guid.NewGuid(),
                Timestamp: DateTime.UtcNow,
                Level: "Info", // بعداً آپدیت می‌شه
                Message: string.Empty,
                ExceptionMessage: null,
                StackTrace: null,
                UserId: GetUserId(context),
                HashedIp: HashIp(GetClientIp(context)),
                Method: request.Method,
                Path: $"{request.Path}{request.QueryString}",
                StatusCode: 0, // بعداً آپدیت می‌شه
                Project: "API",
                DurationMs: null
            );

            try
            {
                await _next(context);
                stopwatch.Stop();

                logDto = logDto with
                {
                    StatusCode = response.StatusCode,
                    DurationMs = stopwatch.Elapsed.TotalMilliseconds,
                    Level = GetLogLevel(response.StatusCode),
                    Message = $"Request completed: {request.Method} {logDto.Path}"
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logDto = logDto with
                {
                    StatusCode = 500,
                    DurationMs = stopwatch.Elapsed.TotalMilliseconds,
                    Level = "Error",
                    Message = $"Request failed: {request.Method} {logDto.Path}",
                    ExceptionMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };

                throw; // خطا رو به کلاینت برگردون
            }
            finally
            {
                _ = Task.Run(async () =>
                {
                    try { await logService.LogAsync(logDto); }
                    catch (Exception ex)
                    {
                        // لاگ خطا در لاگ‌گیری (اختیاری)
                        System.Diagnostics.Debug.WriteLine($"Log failed: {ex.Message}");
                    }
                });
            }
        }
        private static string GetUserId(HttpContext context)
        {
            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            return claim?.Value ?? "Anonymous";
        }
        private static string? GetClientIp(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip) || ip == "::1") ip = "127.0.0.1";

            // پشت پروکسی (Cloudflare, Nginx, ...)
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
                ip = forwarded.ToString().Split(',')[0].Trim();

            if (context.Request.Headers.TryGetValue("X-Real-IP", out var real))
                ip = real.ToString();

            return ip;
        }

        private static string? HashIp(string? ip)
        {
            if (string.IsNullOrEmpty(ip)) return null;

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(ip + "RESTAURANT_APP_SALT_2025"));
            return Convert.ToBase64String(bytes);
        }

        private static string GetLogLevel(int statusCode) => statusCode switch
        {
            >= 200 and < 300 => "Info",
            >= 400 and < 500 => "Warning",
            >= 500 => "Error",
            _ => "Info"
        };

        private static bool ShouldLog(string level, int? statusCode)
        {
            // فقط خطاها و 4xx/5xx رو لاگ کن (برای جلوگیری از حجم زیاد)
            return level is "Error" or "Warning" || (statusCode >= 400);
        }
    }
}
