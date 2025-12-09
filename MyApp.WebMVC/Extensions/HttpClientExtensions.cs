using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace MyApp.WebMVC.Extensions;

public static class HttpClientExtensions
{


    private static HttpClient AddJwtToken(this HttpClient client, HttpContext context)
    {
        var token = context.Items["__LatestAccessToken"] as string
                    ?? context.Request.Cookies["access_token"];

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public static HttpClient WithJwt(this HttpClient client, Controller controller)
        => client.AddJwtToken(controller.HttpContext);

    public static HttpClient WithJwt(this HttpClient client, HttpContext context)
            => client.AddJwtToken(context); // مستقیم از متد private استفاده می‌کنه
}