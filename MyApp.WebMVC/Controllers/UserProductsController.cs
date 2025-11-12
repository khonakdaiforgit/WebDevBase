using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.DTOs;
using MyApp.WebMVC.Helper;
using MyApp.WebMVC.Models;


namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class UserProductsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        public UserProductsController(
            IHttpClientFactory httpClientFactory,
            IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var userId = this.GetCurrentUserId();

            var response = await _httpClient.GetAsync($"FileLink/GetUserFileLinks/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var fileLinks = await response.Content.ReadFromJsonAsync<List<FileLinkDto>>();
                var viewModels = _mapper.Map<List<FileLinkViewModel>>(fileLinks);
                viewModels = viewModels.OrderByDescending(c => c.CreatedAt).ToList();
                return View(viewModels);
            }
            return View();
        }

        [HttpGet]
        public IActionResult NewProduct()
        {
            var model = new FileLinkViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> NewProduct(FileLinkViewModel model)
        {
            if (ModelState.IsValid)
            {

                var userId = this.GetCurrentUserId();


                var dto = new FileLinkDto
                {
                    Url = model.Url,
                    FileDescription = model.FileDescription,
                    Price = model.Price,
                    Status = Domain.Enums.FileStatus.Active,
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow,
                    UserId = userId
                };

                var response = await _httpClient.PostAsJsonAsync($"FileLink/{userId}", dto);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditProduct(Guid fileLinkId)
        {
            var userId = this.GetCurrentUserId();
            var response = await _httpClient.GetAsync($"FileLink/GetFileLink/{fileLinkId}");

            if (response.IsSuccessStatusCode)
            {
                var fileLink = await response.Content.ReadFromJsonAsync<FileLinkDto>();
                if (fileLink.UserId != userId)
                {
                    TempData["ErrorMessage"] = "You are not authorized to edit this product.";
                    return RedirectToAction(nameof(Index));
                }

                var model = _mapper.Map<FileLinkViewModel>(fileLink);
                return View(model);
            }

            TempData["ErrorMessage"] = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(FileLinkViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = this.GetCurrentUserId();

                var responseFileLink = await _httpClient.GetAsync($"FileLink/GetFileLink/{model.Id}");
                var fileLink = await responseFileLink.Content.ReadFromJsonAsync<FileLinkDto>();

                fileLink.Url = model.Url;
                fileLink.FileDescription = model.FileDescription;
                fileLink.Price = model.Price;

                var response = await _httpClient.PutAsJsonAsync($"FileLink/{model.Id}", fileLink);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Product updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["ErrorMessage"] = "An error occurred while updating the product.";
            }

            return View(model);
        }

        // Action to delete a product
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(Guid fileLinkId)
        {
            var userId = this.GetCurrentUserId();
            var response = await _httpClient.DeleteAsync($"FileLink/{fileLinkId}?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "An error occurred while deleting the product.";
            return RedirectToAction(nameof(Index));
        }

        // Action to deactivate a product
        [HttpPost]
        public async Task<IActionResult> DeactivateProduct(Guid fileLinkId)
        {
            var userId = this.GetCurrentUserId();
            var response = await _httpClient.PostAsync($"FileLink/Deactivate/{fileLinkId}?userId={userId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deactivated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "An error occurred while deactivating the product.";
            return RedirectToAction(nameof(Index));
        }

        // Action to deactivate a product
        [HttpPost]
        public async Task<IActionResult> ActivateProduct(Guid fileLinkId)
        {
            var userId = this.GetCurrentUserId();
            var response = await _httpClient.PostAsync($"FileLink/Activate/{fileLinkId}?userId={userId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product deactivated successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "An error occurred while deactivating the product.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ProductTransaction(Guid fileLinkId, int page = 1, int pageSize = 10)
        {
            var userId = this.GetCurrentUserId();

            var response = await _httpClient.GetAsync($"FileLink/GetFileLinksTransaction/{fileLinkId}?page={page}&pageSize={pageSize}");

            var modelRes = await response.Content.ReadFromJsonAsync<PagedResultDto<TransactionDto>>();

            var responseFileLink = await _httpClient.GetAsync($"FileLink/GetFileLink/{fileLinkId}");
            var fileLink = await responseFileLink.Content.ReadFromJsonAsync<FileLinkDto>();

            ViewBag.fileLink = _mapper.Map<FileLinkDto, FileLinkViewModel>(fileLink);

            var model = _mapper.Map<PagedViewModel<TransactionViewModel>>(modelRes);


            return View(model);
        }
    }
}
