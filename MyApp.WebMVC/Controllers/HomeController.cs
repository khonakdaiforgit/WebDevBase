using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Views.Home.ViewModels;

namespace MyApp.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _http;
        private readonly IMapper _mapper;

        public HomeController(
            IHttpClientFactory clientFactory,
            IMapper mapper)
        {
            _http = clientFactory;
            _mapper = mapper;   
        }

        protected HttpClient Api() => _http.CreateClient("ApiClient");

        public async Task<IActionResult> Index()
        {
            var dto = await Api().GetFromJsonAsync<PublicRestaurantDto>("api/public/info");

            var viewModel = dto is not null
                ? _mapper.Map<HomeIndexViewModel>(dto)
                : new HomeIndexViewModel { RestaurantName = "Pearl" };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}