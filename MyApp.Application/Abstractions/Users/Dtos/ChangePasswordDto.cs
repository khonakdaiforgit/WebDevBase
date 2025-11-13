namespace MyApp.Application.Abstractions.Users.Dtos
{
    public record ChangePasswordDto(string CurrentPassword, string NewPassword);
}