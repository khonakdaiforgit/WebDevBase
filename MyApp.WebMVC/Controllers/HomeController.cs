using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs;
using MyApp.Domain.Entities;
using MyApp.WebMVC.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace MyApp.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory,
            IMapper mapper)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("publicApi");
            _mapper = mapper;

        }

        [HttpGet]
        public IActionResult Index(string? refCode)
        {
            if (!string.IsNullOrEmpty(refCode))
            {
                if (Regex.IsMatch(refCode, "^[A-Z0-9]{12}$"))
                {
                    // تنظیم کوکی با عمر 7 روز
                    CookieOptions options = new CookieOptions
                    {
                        HttpOnly = true,       // فقط سمت سرور قابل دسترسی
                        Secure = true,         // فقط HTTPS
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddDays(365)
                    };

                    Response.Cookies.Append("ReferralCode", refCode, options);

                    return RedirectToAction("Register", "Account");
                }
                else
                {
                    return BadRequest("Invalid referral code format.");
                }
            }

            return View();
        }

        public IActionResult AffiliateProgram()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Fees()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpGet("{uniqueCode:regex(^[[A-Z0-9]]{{10}}$)}")]
        public async Task<IActionResult> FileSale(string uniqueCode)
        {
            var response = await _httpClient.GetAsync($"FileLink/GetFileLinkByCode/{uniqueCode}");

            if (response.IsSuccessStatusCode)
            {
                var fileLink = await response.Content.ReadFromJsonAsync<FileLinkDto>();
                if (fileLink.Status == Domain.Enums.FileStatus.Active)
                {
                    var viewModels = _mapper.Map<FileLinkViewModel>(fileLink);
                    viewModels.Url = null;
                    return View(viewModels);
                }
                return NotFound();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]

        public async Task<IActionResult> PayAsync(Guid fileLinkId)
        {
            var response = await _httpClient.GetAsync($"FileLink/NewPay/{fileLinkId}");
            if (response.IsSuccessStatusCode)
            {
                var payUrl = await response.Content.ReadFromJsonAsync<string>();

                return Redirect(payUrl); // کاربر مستقیم به صفحه پرداخت منتقل میشه
            }
            return NotFound();
        }

        [HttpPost("Home/callback")]
        public async Task<IActionResult> CallbackAsync([FromBody] NowPaymentsCallback callback)
        {
            if (!callback.order_id.StartsWith("ORDER"))
            {
                return Unauthorized();
            }

            if (callback.payment_status == "finished")
            {
                var transactionID = Guid.Parse(callback.order_id.Split('_')[1]);

                Console.WriteLine($"{_httpClient.BaseAddress}FileLink/SuccessTransaction/{transactionID}");
                var response = await _httpClient.PostAsJsonAsync($"FileLink/SuccessTransaction/{transactionID}", callback);
                if (response.IsSuccessStatusCode)
                {

                    var FileUrl = await response.Content.ReadFromJsonAsync<string>();



                    return RedirectToAction("DownloadFile", new { FileUrl = FileUrl });
                }


                // ✅ سفارش با موفقیت پرداخت شده
                // اینجا می‌تونی وضعیت سفارش رو در دیتابیس به "پرداخت‌شده" تغییر بدی
                // یا ایمیل بفرستی یا دسترسی بدهی
                //return new JsonResult(callback);
            }

            return Ok(); // حتماً OK برگردونی
        }



        public IActionResult DownloadFile(string FileUrl)
        {
            if (string.IsNullOrEmpty(FileUrl))
            {
                return NotFound();
            }
            // فرض می‌کنیم که FileUrl یک لینک مستقیم به فایل است
            ViewBag.FileUrl = FileUrl;
            return View();
        }

        [HttpGet]
        public IActionResult GetFile(string FileUrl)
        {
            var content = $"File URL : {FileUrl}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            return File(bytes, "text/plain", "Crypto File.txt");
        }

        [HttpGet("Home/TestCallback")]
        public async Task<IActionResult> TestCallback()
        {
            var testCallback = new NowPaymentsCallback
            {
                payment_id = "99999999",
                payment_status = "finished",
                pay_address = "0xTestAddress",
                price_amount = 100,
                price_currency = "usd",
                pay_amount = 0.01m,
                pay_currency = "eth",
                order_id = "ORDER_",
                order_description = "Test order",
                purchase_id = "test-purchase",
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
                network = "ethereum"
            };

            return await CallbackAsync(testCallback);
        }


    }
}
