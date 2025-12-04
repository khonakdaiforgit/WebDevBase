using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Subscribers.Dtos;
using MyApp.Application.Common;
using MyApp.WebMVC.Extensions;
using MyApp.WebMVC.Views.Subscribers.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class SubscribersController : Controller
    {
        private readonly IHttpClientFactory _http;

        public SubscribersController(IHttpClientFactory http)
        {
            _http = http;
        }

        private HttpClient Api() => _http.CreateClient("ApiClient").WithJwt(this);
        private HttpClient PublicApi() => _http.CreateClient("ApiClient"); // بدون JWT برای AllowAnonymous



        // ========================================
        // Public: Subscribe form (POST)
        // ========================================
        [AllowAnonymous]
        [HttpPost("subscribe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || !new EmailAddressAttribute().IsValid(dto.Email))
                return Json(new { success = false, message = "Please enter a valid email address." });

            var content = new StringContent(JsonSerializer.Serialize(dto.Email), Encoding.UTF8, "application/json");
            var response = await PublicApi().PostAsync("api/subscribers/subscribe", content);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Thank you! Please check your email to confirm your subscription." });
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return Json(new { success = false, message = "You're already subscribed!" });

            return Json(new { success = false, message = "Subscription failed. Please try again later." });
        }

        // ========================================
        // Dashboard: List of subscribers
        // ========================================
        public async Task<IActionResult> Index(int page = 1, int pageSize = 50)
        {
            var response = await Api().GetAsync($"api/subscribers?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load subscribers list.";
                return View(new SubscriberListViewModel());
            }

            var result = await response.Content.ReadFromJsonAsync<PagedResult<SubscriberDto>>();
            var model = new SubscriberListViewModel
            {
                Items = result!.Items.Select(x => new SubscriberItemViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    SubscribedAt = x.SubscribedAt,
                    IsActive = x.IsActive
                }).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return View(model);
        }

        // ========================================
        // Remove subscriber (Admin)
        // ========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Email is required.";
                return RedirectToAction(nameof(Index));
            }

            var response = await Api().DeleteAsync($"api/subscribers/{Uri.EscapeDataString(email)}");

            if (response.IsSuccessStatusCode)
                TempData["Success"] = $"Subscriber {email} has been removed.";
            else
                TempData["Error"] = $"Failed to remove {email}.";

            return RedirectToAction(nameof(Index));
        }

        // ========================================
        // Export active emails as CSV
        // ========================================
        public async Task<IActionResult> Export()
        {
            var response = await Api().GetAsync("api/subscribers/emails");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to export emails.";
                return RedirectToAction(nameof(Index));
            }

            var emails = await response.Content.ReadFromJsonAsync<List<string>>();
            var csv = new StringBuilder();
            csv.AppendLine("Email");
            foreach (var email in emails!)
                csv.AppendLine(email);

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"subscribers_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        }


        // ========================================
        // Public: Confirm subscription page
        // ========================================
        [AllowAnonymous]
        [Route("subscribers/confirm")]
        public async Task<IActionResult> Confirm(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return View("ConfirmResult", new ConfirmResultViewModel { Success = false, Message = "Invalid confirmation link." });

            var response = await PublicApi().GetAsync($"api/subscribers/confirm?token={Uri.EscapeDataString(token)}");

            var model = new ConfirmResultViewModel
            {
                Success = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode
                    ? "Your subscription has been confirmed successfully! 🎉"
                    : "This confirmation link is invalid or has expired."
            };

            return View("ConfirmResult", model);
        }

        // ========================================
        // Public: Unsubscribe page
        // ========================================
        [AllowAnonymous]
        [Route("subscribers/unsubscribe")]
        public async Task<IActionResult> Unsubscribe(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return View("UnsubscribeResult", new UnsubscribeResultViewModel { Success = false, Message = "Invalid unsubscribe link." });

            var response = await PublicApi().GetAsync($"api/subscribers/unsubscribe?token={Uri.EscapeDataString(token)}");

            var model = new UnsubscribeResultViewModel
            {
                Success = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode
                    ? "You have been successfully unsubscribed from our newsletter."
                    : "This unsubscribe link is invalid or has already been used."
            };

            return View("UnsubscribeResult", model);
        }
    }
}