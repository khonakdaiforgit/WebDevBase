using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Restaurants;
using MyApp.Application.Abstractions.Restaurants.Dtos;

namespace MyApp.WebAPI.Controllers
{
    [ApiController]
    [Route("api/public")]
    public class PublicRestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public PublicRestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet("info")]
        [ResponseCache(Duration = 60)] // فقط 60 ثانیه کش بشه — همیشه تازه باشه
        public async Task<ActionResult<PublicRestaurantDto>> GetPublicInfo()
        {
            var restaurant = await _restaurantService.GetPublicInfo(); // یا بر اساس دامنه
            if (restaurant == null) return NotFound();

            return Ok(restaurant);
        }
    }
}
