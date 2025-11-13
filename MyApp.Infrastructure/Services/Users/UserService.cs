using MyApp.Application.Abstractions.Authorization;
using MyApp.Application.Abstractions.Users;
using MyApp.Application.Abstractions.Users.Dtos;
using MyApp.Application.Common;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Common;
using MyApp.Infrastructure.Repositories.Interface;

namespace MyApp.Infrastructure.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthorizationService _authz;

        public UserService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IAuthorizationService authz)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _authz = authz;
        }

        public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : MapToDto(user);
        }

        public async Task DeactivateAsync(Guid targetUserId, Guid callerUserId)
        {
            if (!await _authz.IsOwnerAsync(callerUserId))
                throw new UnauthorizedAccessException("Only project owner can deactivate users.");

            var user = await _userRepository.GetByIdAsync(targetUserId)
                ?? throw new KeyNotFoundException("User not found.");

            user.Deactivate();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(Guid targetUserId, Guid callerUserId)
        {
            if (!await _authz.IsOwnerAsync(callerUserId))
                throw new UnauthorizedAccessException("Only project owner can activate users.");

            var user = await _userRepository.GetByIdAsync(targetUserId)
                ?? throw new KeyNotFoundException("User not found.");

            user.Activate();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<UserDto>> GetListAsync(int page = 1, int pageSize = 20)
        {
            var skip = (page - 1) * pageSize;

            var users = await _userRepository.GetPagedAsync(
                filter: null,
                orderBy: u => u.CreatedAt,
                orderByDescending: true,
                page: page,
                pageSize: pageSize);

            var dtos = users.Items.Select(MapToDto).ToList();

            return new PagedResult<UserDto>(dtos, users.TotalCount, page, pageSize);
        }

        private UserDto MapToDto(User user) => new(
            Id: user.Id,
            Email: user.Email,
            Role: user.Role,
            IsActive: user.IsActive,
            CreatedAt: user.CreatedAt);
    }
}
