using MyApp.Domain.Entities;
using MyApp.Domain.Interfaces;
using MyApp.Domain.Interfaces.Services;

namespace MyApp.Application.UseCases.User
{
    public class SeedAdminUseCase
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher _hasher;

        public SeedAdminUseCase(IUserRepository userRepo, IPasswordHasher hasher)
        {
            _userRepo = userRepo;
            _hasher = hasher;
        }

        public async Task ExecuteAsync()
        {
            var exists = await _userRepo.InsertedAdmin();
            if (exists) return;

            var admin = new Domain.Entities.User
            {
                Email = "admin@restaurant.com",
                PasswordHash = _hasher.Hash("123456"),
                Role = UserRole.Admin,
                IsActive = true
            };

            await _userRepo.AddAsync(admin);
        }
    }
}
