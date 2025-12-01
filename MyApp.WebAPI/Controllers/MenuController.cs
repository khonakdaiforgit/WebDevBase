using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Application.Abstractions.Menus;
using MyApp.Application.Abstractions.Menus.Dtos;
using MyApp.Application.Abstractions.Restaurants;
using MyApp.WebAPI.Extensions;
using System.ComponentModel.DataAnnotations;

namespace MyApp.WebAPI.Controllers
{
    [Authorize]
    [Route("api/menu")]
    [ApiController]
    [Produces("application/json")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;

        public MenuController(
            IMenuService menuService,
            IRestaurantService restaurantService)
        {
            _menuService = menuService;
            _restaurantService = restaurantService;
        }

        private Guid UserId => this.GetUserId();

        // ====================== Category Endpoints ======================

        /// <summary>
        /// ایجاد دسته‌بندی جدید منو
        /// </summary>
        [HttpPost("categories")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateCategoryDto request)
        {
            var dto = new CreateCategoryDto(
                Name: request.Name,
                Order: request.Order
            );

            var categoryId = await _menuService.CreateCategoryAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { categoryId }, categoryId);
        }

        /// <summary>
        /// بروزرسانی دسته‌بندی
        /// </summary>
        [HttpPut("categories/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryDto request)
        {
            var dto = new UpdateCategoryDto(
                Id: categoryId,
                Name: request.Name,
                Order: request.Order
            );

            await _menuService.UpdateCategoryAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// حذف دسته‌بندی (فقط اگر خالی باشد)
        /// </summary>
        [HttpDelete("categories/{categoryId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            await _menuService.DeleteCategoryAsync(categoryId);
            return NoContent();
        }

        /// <summary>
        /// دریافت یک دسته‌بندی با آیتم‌های آن
        /// </summary>
        [HttpGet("categories/{categoryId}")]
        [ProducesResponseType(typeof(MenuCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MenuCategoryDto>> GetCategory(Guid categoryId)
        {
            var category = await _menuService.GetCategoryAsync(categoryId);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPatch("categories/{categoryId}/order")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MoveCategoryOrder(Guid categoryId, bool isUp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _menuService.CategoryMoveOrderAsync(categoryId, isUp);
            return NoContent();
        }



        // ====================== Item Endpoints ======================

        [HttpGet("items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetMenuItem(Guid itemId)
        {

            var dto = await _menuService.GetItemAsync(itemId);
            return Ok(dto);
        }


        /// <summary>
        /// ایجاد آیتم جدید در منو
        /// </summary>
        [HttpPost("items")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> CreateMenuItem([FromBody] CreateMenuItemDto request)
        {
            var dto = new CreateMenuItemDto(
                Name: request.Name,
                Description: request.Description ?? string.Empty,
                Price: request.Price,
                ImageUrl: request.ImageUrl ?? string.Empty,
                IsAvailable: request.IsAvailable,
                CategoryId: request.CategoryId
            );

            var itemId = await _menuService.CreateItemAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { categoryId = request.CategoryId }, itemId);
        }

        /// <summary>
        /// بروزرسانی آیتم منو
        /// </summary>
        [HttpPut("items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateMenuItem(Guid itemId, [FromBody] UpdateMenuItemDto request)
        {
            var dto = new UpdateMenuItemDto(
                Id: itemId,
                Name: request.Name,
                Description: request.Description,
                Price: request.Price,
                ImageUrl: request.ImageUrl,
                IsAvailable: request.IsAvailable
            );

            await _menuService.UpdateItemAsync(dto);
            return NoContent();
        }

        /// <summary>
        /// حذف آیتم منو
        /// </summary>
        [HttpDelete("items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteMenuItem(Guid itemId)
        {
            await _menuService.DeleteItemAsync(itemId);
            return NoContent();
        }

        /// <summary>
        /// تغییر وضعیت موجود/ناموجود بودن (تغییر سریع)
        /// </summary>
        [HttpPatch("items/{itemId}/toggle-availability")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ToggleAvailability(Guid itemId)
        {
            await _menuService.ToggleAvailabilityAsync(itemId);
            return NoContent();
        }

        [HttpPatch("items/{categoryId}/order")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MoveItemOrder(Guid categoryId, Guid itemId, bool isUp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _menuService.ItemMoveOrderAsync(categoryId, itemId, isUp);
            return NoContent();
        }

        // ====================== دریافت کل منو (اختیاری برای داشبورد) ======================

        /// <summary>
        /// دریافت تمام دسته‌بندی‌ها و آیتم‌ها (برای نمایش در داشبورد)
        /// </summary>
        [HttpGet("full")]
        public async Task<ActionResult<IReadOnlyList<MenuCategoryDto>>> GetFullMenu()
        {
            // این متد رو بعداً به IMenuService اضافه می‌کنیم یا از Repository مستقیم می‌گیریم
            // فعلاً برای نمونه از یک سرویس فرضی استفاده می‌کنیم
            var categories = await _menuService.GetAllCategoriesWithItemsAsync();
            return Ok(categories);

            //return Ok(new List<MenuCategoryDto>()); // موقت
        }

        // ====================== Helper ======================
        private async Task<Guid> GetMainRestaurantIdAsync()
        {
            var restaurant = await _restaurantService.GetMainRestaurantAsync();
            return restaurant.Id;
        }
    }
}