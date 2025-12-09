using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Contacts.Dtos;
using MyApp.Application.Common;
using MyApp.WebMVC.Extensions;
using MyApp.WebMVC.Views.ContactMessages.ViewModels;
using System.Text.Json;

namespace MyApp.WebMVC.Areas.Admin.Controllers
{
    [Authorize] 
    public class ContactMessagesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ContactMessagesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient Api => _httpClientFactory.CreateClient("ApiClient").WithJwt(this);

        public async Task<IActionResult> Index(bool? onlyUnread = null, int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var url = $"api/contact-messages?onlyUnread={onlyUnread}&page={page}&pageSize={pageSize}";
            var response = await Api.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Unable to load messages. Please try again.";
                return View(new ContactMessagesIndexViewModel());
            }

            var json = await response.Content.ReadAsStringAsync();
            var pagedResult = JsonSerializer.Deserialize<PagedResult<ContactMessageDto>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var viewModel = new ContactMessagesIndexViewModel
            {
                Messages = pagedResult!.Items.Select(dto => new ContactMessageViewModel
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Email = dto.Email,
                    Message = dto.Message,
                    SentAt = dto.SentAt,
                    IsRead = dto.IsRead
                }).ToList(),
                TotalCount = pagedResult.TotalCount,
                CurrentPage = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                OnlyUnread = onlyUnread
            };

            ViewBag.OnlyUnread = onlyUnread;
            return View(viewModel);
        }

        // POST: /Admin/ContactMessages/MarkAsRead/123
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var response = await Api.PatchAsync($"api/contact-messages/{id}/mark-as-read", null);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Message marked as read.";

            return RedirectToAction(nameof(Index), new { onlyUnread = ViewBag.OnlyUnread });
        }
    }
}