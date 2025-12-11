using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebMVC.Views.Shared.ViewModels;

namespace MyApp.WebMVC.Controllers.Base
{
    public class BaseController : Controller
    {
        private readonly IHttpClientFactory _http;

        public BaseController(
        IHttpClientFactory clientFactory)
        {
            _http = clientFactory;
        }

        protected HttpClient PublicApi() => _http.CreateClient("ApiClient");

        private static SharedLayoutViewModel? _cachedLayoutData;
        private static DateTime _cacheExpiry = DateTime.MinValue;

        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (DateTime.UtcNow > _cacheExpiry || _cachedLayoutData == null)
                {
                    var publicDto = await PublicApi().GetFromJsonAsync<PublicRestaurantDto>("api/public/info");

                    _cachedLayoutData = new SharedLayoutViewModel
                    {
                        RestaurantName = publicDto.Name,
                        LogoUrl = publicDto.LogoUrl ?? "/img/logo_200px.png",
                        FullAddress = publicDto.Address,
                        Phone = publicDto.Phone,
                        Email = publicDto.Email,
                        IsOpenNow = publicDto.IsOpenNow,
                        TodayHoursDisplay = publicDto.TodayHoursDisplay,
                        WorkingHours = publicDto.WorkingHours,
                        Latitude = publicDto.Latitude,
                        Longitude = publicDto.Longitude,
                    };
                    _cacheExpiry = DateTime.UtcNow.AddMinutes(10);
                }

                ViewData["LayoutData"] = _cachedLayoutData;
            }
            catch (Exception) {
                ViewData["LayoutData"] = _cachedLayoutData ?? new SharedLayoutViewModel();
            }


            base.OnActionExecuting(context);
        }
    }
}
