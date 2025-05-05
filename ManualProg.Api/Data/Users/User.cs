using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.Users;

public class User : Entity<Guid>
{
    public UserRole Role { get; set; } = UserRole.Basic;
    public required string Username { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public Guid? ProfileId { get; set; }
    public virtual Profile? Profile { get; set; }
}
