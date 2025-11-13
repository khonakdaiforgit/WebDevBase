using MyApp.Domain.Interfaces.Common;

namespace MyApp.Domain.Entities;

public class User : IHasId<Guid>
{
    public Guid Id { get;  set; } = Guid.NewGuid();
    public string Email { get;  set; }
    public string PasswordHash { get;  set; }
    public string? RefreshToken { get; set; }

    public UserRole Role { get;  set; } = UserRole.Admin;
    public bool IsActive { get;  set; } = true;
    public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;
    public DateTime RefreshTokenExpiry { get; set; }
    public bool IsProjectOwner { get; set; } = false;

    public void ChangePassword(string newHash) => PasswordHash = newHash;
    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void SetAsProjectOwner() => IsProjectOwner = true;
}

public enum UserRole { Admin, Editor }
