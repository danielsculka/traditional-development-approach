using ManualProg.Api.Data.Users;
using System.Security.Claims;

namespace ManualProg.Api.Features.Auth.Services;

public class CurrentUser : ICurrentUser
{
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;

        if (claimsPrincipal == null || !claimsPrincipal.Identity.IsAuthenticated)
            return;

        Id = Guid.Parse(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Username = claimsPrincipal.FindFirstValue(ClaimTypes.Name)!;
        Role = Enum.Parse<UserRole>(claimsPrincipal.FindFirstValue(ClaimTypes.Role)!);

        var profileId = claimsPrincipal.FindFirstValue(ClaimTypesExtensions.ProfileId);

        if (!string.IsNullOrEmpty(profileId))
            ProfileId = Guid.Parse(profileId);
    }

    public Guid Id { get; }
    public string Username { get; } = string.Empty;
    public UserRole Role { get; }
    public Guid? ProfileId { get; }
}
