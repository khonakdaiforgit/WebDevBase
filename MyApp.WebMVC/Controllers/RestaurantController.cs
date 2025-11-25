using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Extensions;
using MyApp.WebMVC.Views.Restaurant.ViewModels;
using System.Net;
using System.Text.Json;

namespace MyApp.WebMVC.Controllers;

[Authorize]
public class RestaurantController : Controller
{
    private readonly IHttpClientFactory _http;
    private readonly IMapper _mapper;

    public RestaurantController(IHttpClientFactory http, IMapper mapper)
    {
        _http = http;
        _mapper = mapper;
    }

    protected HttpClient Api() => _http.CreateClient("ApiClient").WithJwt(this);

    // GET: /Restaurant/Profile
    public async Task<IActionResult> Profile()
    {
        var response = await Api().GetAsync("api/restaurant/profile");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Could not load restaurant profile.";
            return RedirectToAction("Index", "Dashboard");
        }

        var restaurant = await response.Content.ReadFromJsonAsync<RestaurantDto>();
        var model = _mapper.Map<RestaurantProfileViewModel>(restaurant);

        return View(model);
    }

    // POST: /Restaurant/Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(RestaurantProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var updateDto = new UpdateRestaurantDto(
            Id: model.Id,
            Name: model.Name,
            Address: model.Address,
            Latitude: model.Latitude,
            Longitude: model.Longitude,
            Phone: model.Phone,
            Email: model.Email,
            LogoUrl: model.LogoUrl,
            WorkingHours: new Dictionary<string, TimeRangeDto>() 
        );

        var response = await Api().PutAsJsonAsync("api/restaurant", updateDto);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Restaurant information updated successfully!";
            return RedirectToAction("Profile");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
            foreach (var error in errors ?? new())
                foreach (var msg in error.Value)
                    ModelState.AddModelError(error.Key, msg);
        }
        else
        {
            TempData["Error"] = "An error occurred while updating the profile.";
        }

        return View(model);
    }

    // GET: /Restaurant/WorkingHours
    public async Task<IActionResult> WorkingHours()
    {
        var response = await Api().GetAsync("api/restaurant/profile");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Could not load working hours.";
            return RedirectToAction("Index", "Dashboard");
        }

        var restaurant = await response.Content.ReadFromJsonAsync<RestaurantDto>();
        var model = new WorkingHoursViewModel
        {
            RestaurantId = restaurant.Id,
            // تبدیل از Dictionary<string, TimeRangeDto> به پراپرتی‌های ViewModel
            SundayOpen = GetTimeOrDefault(restaurant.WorkingHours, "Sunday", true),
            SundayClose = GetTimeOrDefault(restaurant.WorkingHours, "Sunday", false),
            MondayOpen = GetTimeOrDefault(restaurant.WorkingHours, "Monday", true),
            MondayClose = GetTimeOrDefault(restaurant.WorkingHours, "Monday", false),
            TuesdayOpen = GetTimeOrDefault(restaurant.WorkingHours, "Tuesday", true),
            TuesdayClose = GetTimeOrDefault(restaurant.WorkingHours, "Tuesday", false),
            WednesdayOpen = GetTimeOrDefault(restaurant.WorkingHours, "Wednesday", true),
            WednesdayClose = GetTimeOrDefault(restaurant.WorkingHours, "Wednesday", false),
            ThursdayOpen = GetTimeOrDefault(restaurant.WorkingHours, "Thursday", true),
            ThursdayClose = GetTimeOrDefault(restaurant.WorkingHours, "Thursday", false),
            FridayOpen = GetTimeOrDefault(restaurant.WorkingHours, "Friday", true),
            FridayClose = GetTimeOrDefault(restaurant.WorkingHours, "Friday", false),
            SaturdayOpen = GetTimeOrDefault(restaurant.WorkingHours, "Saturday", true),
            SaturdayClose = GetTimeOrDefault(restaurant.WorkingHours, "Saturday", false),
        };

        return View(model);
    }

    // POST: /Restaurant/WorkingHours
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WorkingHours(WorkingHoursViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dict = new Dictionary<string, (TimeSpan Open, TimeSpan Close)>
        {
            ["Sunday"] = (model.SundayOpen, model.SundayClose),
            ["Monday"] = (model.MondayOpen, model.MondayClose),
            ["Tuesday"] = (model.TuesdayOpen, model.TuesdayClose),
            ["Wednesday"] = (model.WednesdayOpen, model.WednesdayClose),
            ["Thursday"] = (model.ThursdayOpen, model.ThursdayClose),
            ["Friday"] = (model.FridayOpen, model.FridayClose),
            ["Saturday"] = (model.SaturdayOpen, model.SaturdayClose)
        };

        var dto = new UpdateWorkingHoursDto(model.RestaurantId, dict);

        var response = await Api().PatchAsJsonAsync("api/restaurant/working-hours", dto);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Working hours updated successfully!";
            return RedirectToAction("WorkingHours");
        }

        TempData["Error"] = "Failed to update working hours.";
        return View(model);
    }

    // آپلود لوگو (اختیاری — اگر می‌خوای مستقیم از داشبورد آپلود کنی)
    [HttpPost]
    public async Task<IActionResult> UploadLogo(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Json(new { success = false, message = "No file selected." });

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        content.Add(new StreamContent(stream), "file", file.FileName);

        var response = await Api().PostAsync("api/upload/logo", content); // باید این اندپوینت رو بعداً بسازی

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            var url = result.GetProperty("url").GetString();
            return Json(new { success = true, url });
        }

        return Json(new { success = false, message = "Upload failed." });
    }

    // متد کمکی برای خواندن ساعت‌ها
    private TimeSpan GetTimeOrDefault(Dictionary<string, TimeRangeDto> hours, string day, bool isOpen)
    {
        if (hours.TryGetValue(day, out var range))
            return isOpen ? range.Open : range.Close;

        return isOpen ? new TimeSpan(9, 0, 0) : new TimeSpan(22, 0, 0);
    }
}