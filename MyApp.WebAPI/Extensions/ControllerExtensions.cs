using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyApp.WebAPI.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// استخراج UserId از JWT (Claim: nameidentifier)
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">اگر توکن نامعتبر باشه</exception>
        public static Guid GetUserId(this ControllerBase controller)
        {
            var claim = controller.User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing user ID in token.");

            return userId;
        }
    }
}
