namespace MyApp.Application.Abstractions.Users
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
        bool IsProjectOwner { get; }
    }
}
