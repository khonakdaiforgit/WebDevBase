using MyApp.Domain.Entities;

namespace MyApp.Application.Abstractions.Users.Dtos
{
    public record UserDto(
        Guid Id,
        string Email,
        UserRole Role,
        bool IsActive,
        DateTime CreatedAt);
}