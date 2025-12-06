// File: Controllers/GalleryController.cs
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Galleries.Dtos;
using MyApp.Application.Common;
using MyApp.WebMVC.Extensions;
using MyApp.WebMVC.Views.Gallery.ViewModels;
using System.Text.Json;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class GalleryController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IMapper _mapper;

        public GalleryController(IHttpClientFactory http, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
        }

        private HttpClient Api() => _http.CreateClient("ApiClient").WithJwt(this);
        private HttpClient PublicApi() => _http.CreateClient("ApiClient");
        [AllowAnonymous]
        public async Task<IActionResult> Show()
        {
            var response = await PublicApi().GetAsync("api/gallery/public");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Gallery is temporarily unavailable.";
                return View(new List<PublicGalleryItemViewModel>());
            }

            var items = await response.Content.ReadFromJsonAsync<List<GalleryItemDto>>();

            var model = items?
                .Where(x => x.IsVisible)
                .Select(x => new PublicGalleryItemViewModel
                {
                    ImageUrl = x.ImageUrl,
                    Caption = x.Caption ?? string.Empty,
                    UploadDate = x.UploadDate.ToLocalTime()
                })
                .OrderByDescending(x => x.UploadDate)
                .ToList() ?? new List<PublicGalleryItemViewModel>();

            return View(model);
        }

        // GET: /Gallery
        public async Task<IActionResult> Index()
        {
            var response = await Api().GetAsync("api/gallery/all"); // بدون ارور + همه رو می‌گیره

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to load gallery photos.";
                return View(new GalleryIndexViewModel());
            }

            var list = await response.Content.ReadFromJsonAsync<List<GalleryItemDto>>();
            var model = new GalleryIndexViewModel
            {
                Items = list!.Select(x => new GalleryItemViewModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Caption = x.Caption,
                    UploadDate = x.UploadDate,
                    IsVisible = x.IsVisible
                }).OrderByDescending(x => x.UploadDate).ToList()
            };

            return View(model);
        }

        // GET: /Gallery/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Gallery/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGalleryItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please select a photo.");
                return View(model);
            }

            // آپلود تصویر (مثل منو) — استفاده از همون متد UploadImageAsync
            var imageUrl = await model.ImageFile.UploadImageAsync("gallery");

            if (imageUrl == null)
            {
                TempData["Error"] = "Image upload failed. Only JPG, PNG, WebP allowed.";
                return View(model);
            }

            var uploadDto = new
            {
                ImageUrl = imageUrl,
                Caption = model.Caption?.Trim() ?? string.Empty
            };

            var response = await Api().PostAsJsonAsync("api/gallery/upload", uploadDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Photo uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to save photo to gallery.";
            return View(model);
        }

        // GET: /Gallery/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await Api().GetAsync($"api/gallery/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Photo not found.";
                return RedirectToAction(nameof(Index));
            }

            var dto = await response.Content.ReadFromJsonAsync<GalleryItemDto>();
            var model = new EditGalleryItemViewModel
            {
                Id = dto!.Id,
                Caption = dto.Caption,
                IsVisible = dto.IsVisible,
                CurrentImageUrl = dto.ImageUrl,
                UploadDate = dto.UploadDate
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditGalleryItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string? newImageUrl = model.CurrentImageUrl;

            // اگر عکس جدید آپلود شده
            if (model.NewImageFile != null && model.NewImageFile.Length > 0)
            {
                newImageUrl = await model.NewImageFile.UploadImageAsync("gallery");
                if (newImageUrl == null)
                {
                    TempData["Error"] = "Failed to upload new photo.";
                    return View(model);
                }

                // حذف عکس قدیمی از سرور
                if (!string.IsNullOrEmpty(model.CurrentImageUrl))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", model.CurrentImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
            }

            var dto = new UpdateGalleryItemDto(model.Id, model.Caption, newImageUrl, model.IsVisible);
        

            var response = await Api().PutAsJsonAsync($"api/gallery/{model.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Photo updated successfully!" +
                                      (model.NewImageFile != null ? " New photo replaced." : "");
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to update photo details.";
            return View(model);
        }

        // POST: /Gallery/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await Api().DeleteAsync($"api/gallery/{id}");
            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Photo deleted successfully!";
            else
                TempData["Error"] = "Failed to delete photo.";

            return RedirectToAction(nameof(Index));
        }

        // PATCH: Hide / Show
        [HttpPost]
        public async Task<IActionResult> ToggleVisibility(Guid id, bool makeVisible)
        {
            var url = makeVisible ? $"api/gallery/{id}/show" : $"api/gallery/{id}/hide";
            var response = await Api().PatchAsync(url, null);

            return Json(new { success = response.IsSuccessStatusCode });
        }
    }
}