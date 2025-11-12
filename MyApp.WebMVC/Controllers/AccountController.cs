using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyApp.Application.DTOs;
using MyApp.Application.Helper;
using MyApp.WebMVC.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyApp.WebMVC.Controllers
{
    // WebMvc/Controllers/AccountController.cs
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public AccountController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("publicApi");
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var model = new RegisterUserViewModel()
            {
                Id = Guid.NewGuid()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Check Exist ReferralCode
            var refCode = Request.Cookies["ReferralCode"]; 

            var dto = new RegisterUserDto
            {
                Id = model.Id,
                Password = model.Password,
                RegisteredWithReferralCode = refCode
            };


            var response = await _httpClient.PostAsJsonAsync("user/register", dto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Username"] = model.Id;
                TempData["Password"] = model.Password;

                Response.Cookies.Delete("ReferralCode");

                return RedirectToAction("DownloadCredentials");
            }

            ModelState.AddModelError("", "Registration failed.");
            return View(model);
        }


        [HttpGet]
        public IActionResult DownloadCredentials()
        {
            if (TempData["Username"] == null || TempData["Password"] == null)
            {
                return RedirectToAction("Register");
            }

            var model = new RegisterUserViewModel
            {
                Id = Guid.Parse(TempData["Username"].ToString()),
                Password = TempData["Password"].ToString()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetCredentialsFile(string id, string password)
        {
            var content = $"Username: {id}\nPassword: {password}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            return File(bytes, "text/plain", "LinkSafe Credentials.txt");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine($"ModelState invalid: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Id.ToString()))
            {
                Console.WriteLine("UserId is null or empty");
                ModelState.AddModelError("", "User ID cannot be empty.");
                return View(model);
            }

            var dto = new LoginDto { Id = model.Id, Password = model.Password };
            var response = await _httpClient.PostAsJsonAsync("user/login", dto);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<TokenRequestDto>();
                //Console.WriteLine($"Login successful, AccessToken: {responseContent.AccessToken}");

                var newJwt = new JwtSecurityTokenHandler().ReadJwtToken(responseContent.AccessToken);

                // ذخیره توکن‌ها
                //Response.Cookies.Append("JwtToken", responseContent.AccessToken, new CookieOptions
                //{
                //    HttpOnly = true,
                //    Secure = false, // برای localhost
                //    SameSite = SameSiteMode.Strict,
                //    Expires = newJwt.ValidTo
                //});

                Response.Cookies.Append("RefreshToken", responseContent.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(AppSettingsHelper.refreshTokenExpireAfterMints)
                });

                // دیباگ توکن JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(responseContent.AccessToken);
                var userId = model.Id.ToString();

                // تنظیم Claims
                var claims = jwtToken.Claims.ToList();
                claims.Add(new Claim("JwtToken", responseContent.AccessToken));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(AppSettingsHelper.refreshTokenExpireAfterMints),
                    IssuedUtc = DateTimeOffset.UtcNow
                };

                Console.WriteLine($"ClaimsIdentity.IsAuthenticated before SignIn: {claimsIdentity.IsAuthenticated}");
                Console.WriteLine($"Claims: {string.Join(", ", claims.Select(c => $"{c.Type}: {c.Value}"))}");

                // انجام SignIn
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // دیباگ هدرهای Set-Cookie
                var setCookieHeaders = Response.Headers["Set-Cookie"];
                Console.WriteLine("Set-Cookie headers:");
                if (setCookieHeaders.Any())
                {
                    foreach (var header in setCookieHeaders)
                    {
                        Console.WriteLine($"Set-Cookie: {header}");
                    }
                }
                else
                {
                    Console.WriteLine("No Set-Cookie headers found");
                }

                // دیباگ HttpContext.User
                Console.WriteLine($"HttpContext.User.Identity.IsAuthenticated after SignIn: {HttpContext.User.Identity.IsAuthenticated}");
                Console.WriteLine($"HttpContext.User.Identity.Name: {HttpContext.User.Identity.Name}");
                if (HttpContext.User.Claims.Any())
                {
                    Console.WriteLine($"HttpContext.User.Claims: {string.Join(", ", HttpContext.User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
                }
                else
                {
                    Console.WriteLine("No claims found in HttpContext.User");
                }

                // تاخیر برای اطمینان
                //await Task.Delay(100);
                Console.WriteLine($"HttpContext.User.Identity.IsAuthenticated after delay: {HttpContext.User.Identity.IsAuthenticated}");

                return RedirectToAction("Index", "UserProfile");
            }

            Console.WriteLine($"Login failed: {response.StatusCode}");
            ModelState.AddModelError("", "Login failed.");
            return View(model);
        }


        [HttpGet]
        public IActionResult TestDataProtection([FromServices] IDataProtectionProvider provider)
        {
            try
            {
                var protector = provider.CreateProtector("TestPurpose");
                var protectedData = protector.Protect("TestData");
                var unprotectedData = protector.Unprotect(protectedData);
                return Content($"Protected: {protectedData}, Unprotected: {unprotectedData}");
            }
            catch (Exception ex)
            {
                return Content($"Data Protection error: {ex.Message}");
            }
        }

        [HttpGet]


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("JwtToken");
            Response.Cookies.Delete("RefreshToken");
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }

    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}

