using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;

namespace MyApp.Infrastructure.Seed;

public static class SeedData
{
    public static async Task InitializeAsync(IUnitOfWork unitOfWork)
    {
        if (await unitOfWork.Users.CountAsync() > 0) return;

        var owner = new User
        {
            Email = "owner@restaurantapp.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Owner@123!"),
            Role = UserRole.Admin,
            IsActive = true
        };


        owner.SetAsProjectOwner();

        await unitOfWork.Users.AddAsync(owner);
        await unitOfWork.SaveChangesAsync();
    }
}