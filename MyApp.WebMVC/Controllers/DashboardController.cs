using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Extensions;
using MyApp.WebMVC.Views.Dashboard.ViewModels;

namespace MyApp.WebMVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IMapper _mapper;

        public DashboardController(IHttpClientFactory http, IMapper mapper)
        {
            _http = http;
            _mapper = mapper;
        }
        protected HttpClient Api() => _http.CreateClient("ApiClient").WithJwt(this);

        public async Task<IActionResult> Index()
        {
            try
            {
                var dto = await Api().GetFromJsonAsync<RestaurantDto>("api/restaurant/profile");

                var vm = dto != null
                    ? _mapper.Map<DashboardIndexViewModel>(dto)
                    : null;

                if (vm == null)
                    return RedirectToAction("Index", "Account"); // یا خطا

                return View(vm);
            }
            catch
            {
                TempData["Error"] = "Unable to load dashboard.";
                return View(new DashboardIndexViewModel());
            }
        }
    }
}
