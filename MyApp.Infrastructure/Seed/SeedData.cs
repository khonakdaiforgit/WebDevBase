// Infrastructure/Seed/SeedData.cs
using MyApp.Domain.Entities;
using MyApp.Domain.ValueObjects;
using MyApp.Infrastructure.Common;

namespace MyApp.Infrastructure.Seed;

public static class SeedData
{
    public static async Task InitializeAsync(IUnitOfWork unitOfWork)
    {
        var users = await unitOfWork.Users.GetAllAsync();

        //var usercats = await unitOfWork.MenuCategories.GetAllAsync();
        //var items = await unitOfWork.MenuItems.GetAllAsync();
        //var ccc = items.GroupBy(c => c.CategoryId);

        //foreach (var item in ccc)
        //{
        //    int index = 0;
        //    foreach (var item2 in item)
        //    {
        //        item2.Order = index++;
        //        await unitOfWork.MenuItems.UpdateAsync(item2);
        //    }

        //}
        //foreach (var item in items)
        //{
        //    await unitOfWork.MenuItems.DeleteAsync(item.Id);
        //}
        //var rests = await unitOfWork.Restaurants.GetAllAsync();

        //for (int i = 0; i < users.Count; i++)
        //{
        //    await unitOfWork.Users.DeleteAsync(users[i].Id);
        //}

        //foreach (var res in rests)
        //{
        //    await unitOfWork.Restaurants.DeleteAsync(res.Id);
        //}

        // 1. ایجاد Project Owner (اگر وجود نداشت)
        if (!users.Any(u => u.IsProjectOwner))
        {
            var owner = new User
            {
                Email = "owner@restaurantapp.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("owner@123!"),
                Role = UserRole.Admin,
                IsActive = true,
                IsProjectOwner = true
            };
            await unitOfWork.Users.AddAsync(owner);
        }

        // 2. ایجاد Editor + رستوران نمونه (اگر ادمین معمولی وجود نداشت)
        if (!users.Any(u => u.Role == UserRole.Admin && !u.IsProjectOwner))
        {
            var editor = new User
            {
                Email = "admin@restaurantapp.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123!"),
                Role = UserRole.Admin,
                IsActive = true
            };

            await unitOfWork.Users.AddAsync(editor);
            await unitOfWork.SaveChangesAsync(); // مهم: اول کاربر ذخیره بشه تا Id داشته باشه

            // حالا یک رستوران زیبا و واقعی برای ادمین بسازیم
            var restaurant = new Restaurant
            {
                Name = "The Urban Bistro",
                Address = "123 Downtown Avenue, New York, NY 10001",
                Phone = "+1 (555) 123-4567",
                Email = "hello@urbanbistro.com",
                Mian = true,
                LogoUrl = "https://images.unsplash.com/photo-1517248135467-2c7ed3da9ab0?w=800&h=600&fit=crop",
                OwnerUserId = editor.Id
            };

            // موقعیت جغرافیایی (نیویورک - نزدیک تایمز اسکوئر)
            restaurant.SetLocation(40.758896, -73.985130);

            // Infrastructure/Seed/SeedData.cs — فقط قسمت رستوران

            // SeedData.cs — فقط این قسمت
            var workingHoursData = new Dictionary<string, (TimeSpan Open, TimeSpan Close)>
            {
                { "Monday",    (new TimeSpan(11, 0, 0), new TimeSpan(23, 0, 0)) },
                { "Tuesday",   (new TimeSpan(11, 0, 0), new TimeSpan(23, 0, 0)) },
                { "Wednesday", (new TimeSpan(11, 0, 0), new TimeSpan(23, 0, 0)) },
                { "Thursday",  (new TimeSpan(11, 0, 0), new TimeSpan(23, 0, 0)) },
                { "Friday",    (new TimeSpan(11, 0, 0), new TimeSpan(23, 59, 0)) },
                { "Saturday",  (new TimeSpan(10, 0, 0), new TimeSpan(23, 59, 0)) },
                { "Sunday",    (new TimeSpan(10, 0, 0), new TimeSpan(22, 0, 0)) }
            };

            restaurant.WorkingHours = WorkingHours.Create(workingHoursData);


            await unitOfWork.Restaurants.AddAsync(restaurant);
        }

        // ذخیره نهایی تغییرات
        await unitOfWork.SaveChangesAsync();
    }
}