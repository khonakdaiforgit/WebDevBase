using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.WebMVC.Models.ViewModels;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _http;
        public AccountController(IHttpClientFactory clientFactory)
        {
            _http = clientFactory;
        }
        protected HttpClient Api() => _http.CreateClient("ApiClient");


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var json = JsonSerializer.Serialize(new { model.Email, model.Password });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Api().PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

                // Save tokens in cookies (or localStorage via JS)
                Response.Cookies.Append("access_token", result!.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = result.ExpiresAt
                });

                Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                });

                return Redirect(returnUrl ?? "/Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Login failed. Check email and password.");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Redirect("/Account/Login");
        }

        //// در AccountController.cs — این متد رو اضافه کن
        //[HttpGet]
        //public async Task<IActionResult> KeepAlive()
        //{
        //    // اول سعی کن به API بری با توکن فعلی
        //    var client = _clientFactory.CreateClient("ApiClient");
        //    var response = await client.GetAsync("api/auth/keep-alive");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // توکن هنوز زنده است → هیچ کاری نکن
        //        return Content("OK");
        //    }

        //    // اگر 401 بود → یعنی access token expire شده → refresh کن!
        //    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //    {
        //        var refreshToken = Request.Cookies["refresh_token"];
        //        if (string.IsNullOrEmpty(refreshToken))
        //        {
        //            return RedirectToAction("Login");
        //        }

        //        var refreshContent = new StringContent(
        //            JsonSerializer.Serialize(new { refreshToken }),
        //            Encoding.UTF8, "application/json");

        //        var refreshResponse = await client.PostAsync("api/auth/refresh", refreshContent);

        //        if (refreshResponse.IsSuccessStatusCode)
        //        {
        //            var result = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();

        //            // کوکی‌های جدید رو ست کن
        //            Response.Cookies.Append("access_token", result!.AccessToken, new CookieOptions
        //            {
        //                HttpOnly = true,
        //                Secure = true,
        //                SameSite = SameSiteMode.Strict,
        //                Expires = result.ExpiresAt
        //            });

        //            Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
        //            {
        //                HttpOnly = true,
        //                Secure = true,
        //                SameSite = SameSiteMode.Strict,
        //                Expires = DateTimeOffset.UtcNow.AddDays(30)
        //            });

        //            return Content("REFRESHED");
        //        }
        //    }

        //    // اگر همه چیز شکست → برو لاگین
        //    Response.Cookies.Delete("access_token");
        //    Response.Cookies.Delete("refresh_token");
        //    return RedirectToAction("Login");
        //}
    }
}

