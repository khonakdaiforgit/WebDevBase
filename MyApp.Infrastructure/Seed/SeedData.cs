using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;

namespace MyApp.Infrastructure.Seed;

public static class SeedData
{
    public static async Task InitializeAsync(IUnitOfWork unitOfWork)
    {
        var allUser = await unitOfWork.Users.GetAllAsync();

        if (!allUser.Any(c => c.IsProjectOwner))
        {
            var owner = new User
            {
                Email = "owner@restaurantapp.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("owner@123!"),
                Role = UserRole.Admin,
                IsActive = true
            };

            owner.SetAsProjectOwner();
            await unitOfWork.Users.AddAsync(owner);
        }

        if (!allUser.Any(c => c.Role == UserRole.Admin && !c.IsProjectOwner))
        {
            var editor = new User
            {
                Email = "admin@restaurantapp.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123!"),
                Role = UserRole.Admin,
                IsActive = true
            };
            await unitOfWork.Users.AddAsync(editor);
        }
    }
}