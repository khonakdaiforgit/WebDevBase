using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Newsletters.Dtos;
using MyApp.Application.Common;
using MyApp.WebMVC.Extensions;
using MyApp.WebMVC.Views.Newsletter.ViewModels;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class NewsletterController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IMapper _mapper;

        public NewsletterController(IHttpClientFactory http, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
        }

        protected HttpClient Api() => _http.CreateClient("ApiClient").WithJwt(this);

        // GET: /Newsletter/Index 
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
        {
            var response = await Api().GetAsync($"api/newsletters?page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load newsletters.";
                return View(new List<NewsletterViewModel>());
            }

            var pagedResult = await response.Content.ReadFromJsonAsync<PagedResult<NewsletterListItemDto>>();

            var model = pagedResult?.Items.Select(dto => new NewsletterViewModel
            {
                Id = dto.Id,
                Subject = dto.Subject,
                SentAt = dto.SentAt,
                Status = dto.Status
            }).ToList() ?? new List<NewsletterViewModel>();

            ViewBag.TotalPages = (int)Math.Ceiling((double)(pagedResult?.TotalCount ?? 0) / pageSize);
            ViewBag.CurrentPage = page;

            return View(model);
        }

        // GET: /Newsletter/Create
        public IActionResult Create()
        {
            return View(new CreateNewsletterViewModel());
        }

        // POST: /Newsletter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewsletterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new CreateNewsletterDto(model.Subject, model.Content);
            var response = await Api().PostAsJsonAsync("api/newsletters", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Newsletter created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Failed to create newsletter.";
                return View(model);
            }
        }

        // GET: /Newsletter/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await Api().GetAsync($"api/newsletters/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var dto = await response.Content.ReadFromJsonAsync<NewsletterDto>();
            if (dto == null)
            {
                return NotFound();
            }

            var model = new EditNewsletterViewModel
            {
                Id = dto.Id,
                Subject = dto.Subject,
                Content = dto.Content
            };

            return View(model);
        }

        // POST: /Newsletter/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditNewsletterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new UpdateNewsletterDto(model.Id, model.Subject, model.Content);
            var response = await Api().PutAsJsonAsync($"api/newsletters/{model.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Newsletter updated successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Failed to update newsletter.";
                return View(model);
            }
        }

        // POST: /Newsletter/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await Api().DeleteAsync($"api/newsletters/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Newsletter deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete newsletter.";
            }

            return RedirectToAction("Index");
        }

        // POST: /Newsletter/SendNow/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendNow(Guid id)
        {
            var response = await Api().PostAsync($"api/newsletters/{id}/send-now", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Newsletter sent successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to send newsletter.";
            }

            return RedirectToAction("Index");
        }

        // GET: /Newsletter/Details/{id} - مشاهده جزئیات
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await Api().GetAsync($"api/newsletters/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var dto = await response.Content.ReadFromJsonAsync<NewsletterDto>();
            if (dto == null)
            {
                return NotFound();
            }

            var model = new NewsletterDetailsViewModel
            {
                Id = dto.Id,
                Subject = dto.Subject,
                Content = dto.Content,
                SentAt = dto.SentAt,
                Status = dto.Status,
                SentByUserId = dto.SentByUserId
            };

            return View(model);
        }
    }
}