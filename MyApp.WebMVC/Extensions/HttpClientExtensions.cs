using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace MyApp.WebMVC.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// خودکار Bearer Token رو از HttpContext (Items یا Cookie) می‌گیره و ست می‌کنه
    /// </summary>
    private static HttpClient WithJwt(this HttpClient client, HttpContext context)
    {
        var token = context.Items["__LatestAccessToken"] as string
                    ?? context.Request.Cookies["access_token"];

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    /// <summary>
    /// نسخه کوتاه‌تر برای استفاده در کنترلر (this Controller)
    /// </summary>
    public static HttpClient WithJwt(this HttpClient client, Controller controller)
        => client.WithJwt(controller.HttpContext);
}