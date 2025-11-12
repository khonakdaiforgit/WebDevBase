using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyApp.WebMVC.Helper
{
    public static class ControllerExtensions
    {
        public static Guid GetCurrentUserId(this Controller controller)
        {
            var userIdString = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
                throw new UnauthorizedAccessException("User is not authenticated");

            return Guid.Parse(userIdString);
        }
    }
}
