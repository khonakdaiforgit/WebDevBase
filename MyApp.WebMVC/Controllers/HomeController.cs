using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Controllers.Base;
using MyApp.WebMVC.Views.Home.ViewModels;
using System.Text;
using System.Text.Json;

namespace MyApp.WebMVC.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IHttpClientFactory _http;
        private readonly IMapper _mapper;

        public HomeController(
            IHttpClientFactory clientFactory,
            IMapper mapper) : base(clientFactory)
        {
            _http = clientFactory;
            _mapper = mapper;   
        }

        protected HttpClient Api() => _http.CreateClient("ApiClient");

        public async Task<IActionResult> Index()
        {
            
            var dto = await PublicApi().GetFromJsonAsync<PublicRestaurantDto>("api/public/info");

            var viewModel = dto is not null
                ? _mapper.Map<HomeIndexViewModel>(dto)
                : new HomeIndexViewModel { RestaurantName = "Pearl" };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Contact()
        {
            //var dto = await PublicApi().GetFromJsonAsync<PublicRestaurantDto>("api/public/info");

            //var restaurantInfo = dto is not null
            //    ? _mapper.Map<HomeIndexViewModel>(dto)
            //    : new HomeIndexViewModel { RestaurantName = "Pearl" };

            //ViewBag.rsturantInfo= restaurantInfo;

            return View(new ContactViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            var request = new 
            {
                name = model.Name?.Trim(),
                email = model.Email?.Trim(),
                message = model.Message?.Trim()
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await PublicApi().PostAsync("api/contact-messages/public/submit", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["ContactSuccess"] = "Thank you! Your message has been sent successfully. We'll get back to you soon.";
                return RedirectToAction("Contact");
            }
            else
            {
                TempData["ContactError"] = "Oops! Something went wrong. Please try again later or email us directly.";
                return View(model);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}