using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Menus.Dtos;
using MyApp.WebMVC.Extensions; // برای WithJwt
using MyApp.WebMVC.Views.Menu.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IMapper _mapper;

        public MenuController(IHttpClientFactory http, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
        }

        protected HttpClient Api() => _http.CreateClient("ApiClient").WithJwt(this);

        // GET: /Menu/Index
        public async Task<IActionResult> Index()
        {
            var response = await Api().GetAsync("api/menu/full");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not load menu.";
                return RedirectToAction("Index", "Dashboard");
            }

            var categories = await response.Content.ReadFromJsonAsync<IReadOnlyList<MenuCategoryDto>>();

            var model = categories?
                .Select(categoryDto => new MenuCategoryViewModel
                {
                    Id = categoryDto.Id,
                    Name = categoryDto.Name,
                    Order = categoryDto.Order,
                    Items = categoryDto.Items.Select(itemDto => new MenuItemViewModel
                    {
                        Id = itemDto.Id,
                        Name = itemDto.Name,
                        Description = itemDto.Description ?? string.Empty,
                        Price = itemDto.Price,
                        ImageUrl = itemDto.ImageUrl ?? string.Empty,
                        IsAvailable = itemDto.IsAvailable
                    }).ToList() ?? new List<MenuItemViewModel>()
                })
                .OrderBy(c => c.Order).ToList()
                 ?? new List<MenuCategoryViewModel>(); return View(model);
        }

        // POST: /Menu/ReorderCategories (برای Drag & Drop دسته‌ها)
        [HttpPost]
        public async Task<IActionResult> ReorderCategories([FromBody] List<Guid> orderedIds)
        {
            // فرض کن یک DTO برای بروزرسانی ترتیب داری، یا مستقیم به API بفرست
            var dtos = orderedIds.Select((id, index) => new UpdateCategoryDto(id, null, index)).ToList();
            var response = await Api().PostAsJsonAsync("api/menu/reorder-categories", dtos); // اگر API این رو ساپورت کنه، اضافه کن به MenuController API
            return response.IsSuccessStatusCode ? Ok() : BadRequest();
        }

        // مشابه برای ReorderItems در یک دسته
        [HttpPost]
        public async Task<IActionResult> ReorderItems(Guid categoryId, [FromBody] List<Guid> orderedIds)
        {
            // مشابه بالا، اما برای آیتم‌ها
            // await Api().PostAsJsonAsync($"api/menu/categories/{categoryId}/reorder-items", orderedIds);
            return Ok();
        }


        // GET: /Menu/CreateCategory
        public IActionResult CreateCategory()
        {
            return View(new CreateCategoryViewModel());
        }

        // POST: /Menu/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new CreateCategoryDto(model.Name, model.Order, Guid.Empty); // RestaurantId از API گرفته می‌شه
            var response = await Api().PostAsJsonAsync("api/menu/categories", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category created!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to create category.";
            return View(model);
        }

        // GET: /Menu/EditCategory/{id}
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var response = await Api().GetAsync($"api/menu/categories/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var dto = await response.Content.ReadFromJsonAsync<MenuCategoryDto>();
            var model = new UpdateCategoryViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Order = dto.Order
            };
            return View(model);
        }

        // POST: /Menu/EditCategory/{id}
        [HttpPost]
        public async Task<IActionResult> EditCategory(Guid id, UpdateCategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = new UpdateCategoryDto(id, model.Name, model.Order);
            var response = await Api().PutAsJsonAsync($"api/menu/categories/{id}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category updated!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to update.";
            return View(model);
        }

        // POST: /Menu/DeleteCategory/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var response = await Api().DeleteAsync($"api/menu/categories/{id}");
            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] = response.IsSuccessStatusCode ? "Deleted!" : "Failed to delete.";
            return RedirectToAction("Index");
        }



        // GET: /Menu/CreateItem?categoryId=xxx
        public IActionResult CreateItem(Guid categoryId)
        {
            var model = new CreateMenuItemViewModel
            {
                CategoryId = categoryId
            };
            return View(model);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateItem(CreateMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // اگر عکس آپلود شده، اول به Cloudinary بفرست (مثل UploadLogo)
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                model.ImageUrl = await model.ImageFile.UploadImageAsync("menu-items"); 
                if (string.IsNullOrEmpty(model.ImageUrl))
                {
                    TempData["Error"] = "Image upload failed.";
                    return View(model);
                }
            }

            var dto = new CreateMenuItemDto(
                Name: model.Name,
                Description: model.Description ?? "",
                Price: model.Price,
                ImageUrl: model.ImageUrl ?? "",
                IsAvailable: model.IsAvailable,
                CategoryId: model.CategoryId
            );

            var response = await Api().PostAsJsonAsync("api/menu/items", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Item added successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to add item.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAvailability(Guid id)
        {
            var response = await Api().PatchAsync($"api/menu/items/{id}/toggle-availability", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Availability updated!";
            }
            else
            {
                TempData["Error"] = "Failed to update availability.";
            }

            return RedirectToAction("Index");
        }

        // POST: /Menu/DeleteItem/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var response = await Api().DeleteAsync($"api/menu/items/{id}");
            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] = response.IsSuccessStatusCode ? "Deleted!" : "Failed to delete.";
            return RedirectToAction("Index");
        }


        // GET: /Menu/EditItem/{id}
        public async Task<IActionResult> EditItem(Guid id)
        {
            var response = await Api().GetAsync($"api/menu/items/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Item not found.";
                return RedirectToAction("Index");
            }

            var dto = await response.Content.ReadFromJsonAsync<MenuItemDto>();

            var model = new UpdateMenuItemViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,
                IsAvailable = dto.IsAvailable
            };

            return View(model);
        }

        // POST: /Menu/EditItem/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItem(UpdateMenuItemViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string? newImageUrl = model.ImageUrl; // نگه داشتن عکس فعلی

            // اگر عکس جدید آپلود شده، آپلود کن و URL جدید بگیر
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                newImageUrl = await model.ImageFile.UploadImageAsync("menu-items");
                if (newImageUrl == null)
                {
                    TempData["Error"] = "Image upload failed. Only JPG, PNG, WebP allowed.";
                    return View(model);
                }

                // حذف عکس قدیمی از سرور (اگر وجود داشت)
                if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", model.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }

            var dto = new UpdateMenuItemDto(
                Id: model.Id,
                Name: model.Name,
                Description: model.Description,
                Price: model.Price,
                ImageUrl: newImageUrl,
                IsAvailable: model.IsAvailable
            );

            var response = await Api().PutAsJsonAsync($"api/menu/items/{model.Id}", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Item updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to update item.";
            return View(model);
        }

    }
}