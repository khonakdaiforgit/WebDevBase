using MyApp.Application.DTOs;
using MyApp.Application.Interfaces;
using System.Diagnostics;

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

            var logEntry = new LogEntryDto
            {
                UserId = context.User?.Identity?.Name ?? "Anonymous",
                HashedIp = GetClientIpHashed(context),
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                Method = context.Request.Method,
                Path = context.Request.Path,
                Source = "RequestStart",
                Project = "API"
            };

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // لاگ شروع درخواست
                logService.LogRequestAsync(logEntry);

                await _next(context);

                // لاگ پایان درخواست (موفق)
                stopwatch.Stop();
                logEntry.Message = $"Request completed successfully in {stopwatch.ElapsedMilliseconds}ms";
                logEntry.StatusCode = context.Response.StatusCode;
                logEntry.Source = "RequestEnd";
                logEntry.Level = "Info";
                logService.LogRequestAsync(logEntry);
            }
            catch (Exception ex)
            {
                // لاگ خطا
                stopwatch.Stop();
                logEntry.Message = $"Request failed in {stopwatch.ElapsedMilliseconds}ms";
                logEntry.ExceptionMessage = ex.Message;
                logEntry.StackTrace = ex.StackTrace;
                logEntry.StatusCode = 500;
                logEntry.Source = "RequestError";
                logEntry.Level = "Error";
                logService.LogErrorAsync(logEntry);

                throw; // پرتاب دوباره برای مدیریت خطا
            }
        }

        public string GetClientIpHashed(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();

            // اگر پشت پراکسی هستی:
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            return ip;//HashIp(ip ?? "Unknown");
        }


    }
}
