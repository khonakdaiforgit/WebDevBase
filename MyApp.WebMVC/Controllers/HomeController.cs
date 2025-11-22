using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Views.Home.ViewModels;

namespace MyApp.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMapper _mapper;

        public HomeController(
            IHttpClientFactory clientFactory,
            IMapper mapper)
        {
            _clientFactory = clientFactory;
            _mapper = mapper;   
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("ApiClient");
            var dto = await client.GetFromJsonAsync<PublicRestaurantDto>("api/public/info");

            var viewModel = dto is not null
                ? _mapper.Map<HomeIndexViewModel>(dto)
                : new HomeIndexViewModel { RestaurantName = "The Urban Bistro" };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}