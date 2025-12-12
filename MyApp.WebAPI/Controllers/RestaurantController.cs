using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Restaurants;
using MyApp.Application.Abstractions.Restaurants.Dtos;
using MyApp.WebAPI.Extensions;

namespace MyApp.WebAPI.Controllers
{
    [Authorize]
    [Route("api/restaurant")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        /// <summary>
        /// Get current user's restaurant profile
        /// </summary>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RestaurantDto>> GetProfile()
        {
            var userId = this.GetUserId(); // از Extension که قبلاً داریم

            // اول همه رستوران‌های کاربر رو پیدا کن (معمولاً فقط یکی داره)
            // فرض ما: هر کاربر فقط یک رستوران داره
            var restaurant = await _restaurantService.GetMainRestaurantAsync();

            if (restaurant == null)
                return NotFound("You don't have a restaurant yet. Please create one first.");

            return Ok(restaurant);
        }

        [AllowAnonymous]
        [HttpGet("info")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<PublicRestaurantDto>> GetPublicInfo()
        {
            var restaurant = await _restaurantService.GetPublicInfo(); // یا بر اساس دامنه
            if (restaurant == null) return NotFound();

            return Ok(restaurant);
        }

        // بقیه اندپوینت‌ها (اختیاری برای آینده)
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateRestaurantDto dto)
        {
            var userId = this.GetUserId();
            var id = await _restaurantService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetProfile), new { }, id);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateRestaurantDto dto)
        {
            var userId = this.GetUserId();
            await _restaurantService.UpdateAsync(dto);
            return NoContent();
        }

        [HttpPatch("logo")]
        public async Task<IActionResult> UpdateLogo([FromBody] UpdateLogoDto dto)
        {
            var userId = this.GetUserId();
            await _restaurantService.UpdateLogoAsync(dto);
            return NoContent();
        }

        [HttpPatch("working-hours")]
        public async Task<IActionResult> UpdateWorkingHours([FromBody] UpdateWorkingHoursDto dto)
        {
            var userId = this.GetUserId();
            await _restaurantService.UpdateWorkingHoursAsync(dto);
            return NoContent();
        }

    }
}
