using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.News.Dtos;
using MyApp.Domain.Entities;
using MyApp.WebMVC.Extensions;
using MyApp.WebMVC.Views.News.ViewModels;
using System.Text.RegularExpressions;


namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IMapper _mapper;

        public NewsController(IHttpClientFactory http, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
        }

        protected HttpClient Api() => _http.CreateClient("ApiClient").WithJwt(this);
        private HttpClient PublicApi() => _http.CreateClient("ApiClient");

        // فایل: NewsController.cs (در WebMVC/Controllers)
        // اضافه کن این اکشن جدید (عمومی برای نمایش لیست اخبار)

        [AllowAnonymous]
        public async Task<IActionResult> AllNews(int page = 1, int pageSize = 9)
        {
            const int DefaultPageSize = 9; // می‌تونی تغییر بدی (مثلاً 6 یا 12 برای گرید بهتر)

            var response = await PublicApi().GetAsync("api/news/public");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "News temporarily unavailable.";
                return View(new NewsPublicListViewModel { News = new List<NewsPublicItemViewModel>(), Pagination = new PaginationViewModel() });
            }

            var allPublishedNews = await response.Content.ReadFromJsonAsync<List<NewsListItemDto>>();

            var publishedNews = allPublishedNews?
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishDate)
                .ToList() ?? new List<NewsListItemDto>();

            var totalItems = publishedNews.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)DefaultPageSize);

            // اعمال صفحه‌بندی
            var itemsOnPage = publishedNews
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(dto => new NewsPublicItemViewModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    ImageUrl = dto.ImageUrl ?? string.Empty,
                    PublishDate = dto.PublishDate,
                    Summary = dto.Content.Length > 150
                        ? System.Net.WebUtility.HtmlDecode(Regex.Replace(dto.Content, "<.*?>", string.Empty)).Substring(0, 200) + "..."
                        : System.Net.WebUtility.HtmlDecode(Regex.Replace(dto.Content, "<.*?>", string.Empty))
                })
                .ToList();

            var model = new NewsPublicListViewModel
            {
                News = itemsOnPage,
                Pagination = new PaginationViewModel
                {
                    CurrentPage = page,
                    TotalPages = totalPages,
                    HasPrevious = page > 1,
                    HasNext = page < totalPages
                }
            };

            return View(model);
        }

        // GET: /News/Index
        public async Task<IActionResult> Index()
        {
            var response = await Api().GetAsync("api/news/all");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load news.";
                return RedirectToAction("Index", "Dashboard");
            }

            var newsList = await response.Content.ReadFromJsonAsync<IReadOnlyList<NewsListItemDto>>();

            var model = newsList?.Select(dto => new NewsViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                PublishDate = dto.PublishDate,
                IsPublished = dto.IsPublished
            })
                .OrderByDescending(n => n.PublishDate).ToList() ?? new List<NewsViewModel>();


            return View(model);
        }

        // GET: /News/Create
        public IActionResult Create()
        {
            return View(new CreateNewsViewModel());
        }

        // POST: /News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string? imageUrl = null;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageUrl = await model.ImageFile.UploadImageAsync("news-images");
                if (imageUrl == null)
                {
                    TempData["Error"] = "Image upload failed. Only JPG, PNG, WebP allowed.";
                    return View(model);
                }
            }

            var dto = new CreateNewsDto(
                Title: model.Title,
                Content: model.Content,
                ImageUrl: imageUrl ?? string.Empty
            );

            var response = await Api().PostAsJsonAsync("api/news", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "News created successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to create news.";
            return View(model);
        }

        // GET: /News/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await Api().GetAsync($"api/news/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "News not found.";
                return RedirectToAction("Index");
            }

            var dto = await response.Content.ReadFromJsonAsync<NewsDto>();

            var model = new UpdateNewsViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                IsPublished = dto.IsPublished
            };
            return View(model);
        }

        // POST: /News/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateNewsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string? newImageUrl = model.ImageUrl;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                newImageUrl = await model.ImageFile.UploadImageAsync("news-images");
                if (newImageUrl == null)
                {
                    TempData["Error"] = "Image upload failed. Only JPG, PNG, WebP allowed.";
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", model.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }

            var dto = new UpdateNewsDto(
                Id: model.Id,
                Title: model.Title,
                Content: model.Content,
                ImageUrl: newImageUrl,
                IsPublished: model.IsPublished
            );

            var response = await Api().PutAsJsonAsync($"api/news/{model.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "News updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to update news.";
            return View(model);
        }

        // POST: /News/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await Api().DeleteAsync($"api/news/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "News deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete news.";
            }

            return RedirectToAction("Index");
        }

        // POST: /News/Publish/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(Guid id)
        {
            var response = await Api().PatchAsync($"api/news/{id}/publish", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "News published successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to publish news.";
            }

            return RedirectToAction("Index");
        }

        // POST: /News/Unpublish/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(Guid id)
        {
            var response = await Api().PatchAsync($"api/news/{id}/unpublish", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "News unpublished successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to unpublish news.";
            }

            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        public async Task<IActionResult> Show(Guid id)
        {
            var response = new HttpResponseMessage();

            if (User.Identity.IsAuthenticated)
            {
                response = await Api().GetAsync($"api/news/{id}");
            }
            else
            {
                response = await PublicApi().GetAsync($"api/news/{id}/show");
            }

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }


            var newsDto = await response.Content.ReadFromJsonAsync<NewsDto>();
            if (newsDto == null)
            {
                return NotFound();
            }

            var model = new NewsPublicViewModel
            {
                Id = newsDto.Id,
                Title = newsDto.Title,
                Content = newsDto.Content,
                ImageUrl = newsDto.ImageUrl ?? string.Empty,
                PublishDate = newsDto.PublishDate,
                IsPublished = newsDto.IsPublished
            };

            return View(model);
        }
    }
}