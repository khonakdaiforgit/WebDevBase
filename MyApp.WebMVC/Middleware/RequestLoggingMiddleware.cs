using MyApp.Application.DTOs;
using System.Diagnostics;

namespace MyApp.WebMVC.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient client;
        private readonly string[] _excludedExtensions = { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".woff", ".woff2", ".ttf", ".ico" };
        private readonly string[] _excludedPaths = { "/swagger", "/favicon.ico", "/Dashboard" };
        public RequestLoggingMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
            client = _httpClientFactory.CreateClient("publicApi");
        }

        public async Task InvokeAsync(HttpContext context)
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
                Project = "MVC",
                Timestamp = DateTime.UtcNow,
                Level = "Info"
            };

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // لاگ شروع درخواست
                client.PostAsJsonAsync("logs/request", logEntry);

                //if (!response.IsSuccessStatusCode)
                //{
                //    var errorContent = await response.Content.ReadAsStringAsync();
                //    Console.WriteLine($"Failed to log to API: Status: {response.StatusCode}, Content: {errorContent}");
                //    System.IO.File.AppendAllText("error.log", $"[{DateTime.UtcNow}] Failed to log to API: Status: {response.StatusCode}, Content: {errorContent}\n");
                //}
                //response.EnsureSuccessStatusCode();

                await _next(context);

                // لاگ پایان درخواست (موفق)
                stopwatch.Stop();
                logEntry.Message = $"Request completed successfully in {stopwatch.ElapsedMilliseconds}ms";
                logEntry.StatusCode = context.Response.StatusCode;
                logEntry.Source = "RequestEnd";
                logEntry.Level = "Info";
                client.PostAsJsonAsync("logs/request", logEntry);
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
                try
                {
                    var response = await client.PostAsJsonAsync("logs/error", logEntry);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Failed to log to API: Status: {response.StatusCode}, Content: {errorContent}");
                        System.IO.File.AppendAllText("error.log", $"[{DateTime.UtcNow}] Failed to log to API: Status: {response.StatusCode}, Content: {errorContent}\n");
                    }
                    response.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException httpEx)
                {
                        System.IO.File.AppendAllText("error.log", $"[{DateTime.UtcNow}] Failed to log to API: Status: HttpRequestException, Content: {httpEx.Message}\n");
                }

                throw; // پرتاب دوباره برای هدایت به صفحه خطا
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
