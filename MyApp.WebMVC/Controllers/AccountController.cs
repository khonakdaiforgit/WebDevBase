using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.WebMVC.Models.ViewModels;
using System.Text;
using System.Text.Json;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

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

            var client = _clientFactory.CreateClient("ApiClient"); // ← این مهم است!
            var response = await client.PostAsync("api/auth/login", content);

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
    }
}

