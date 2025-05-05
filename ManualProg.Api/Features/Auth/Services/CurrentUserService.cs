using ManualProg.Api.Data.Users;
using System.Security.Claims;

namespace ManualProg.Api.Features.Auth.Services;

public class CurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        HttpContext ctx = httpContextAccessor.HttpContext;

        if (ctx == null)
            return;

        ClaimsPrincipal user = ctx.User;

        if (user == null)
            return;

        Id = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        UserName = user.FindFirstValue(ClaimTypes.Name)!;
        Role = Enum.Parse<UserRole>(user.FindFirstValue(ClaimTypes.Role)!);

        var profileId = user.FindFirstValue(ClaimTypesExtensions.ProfileId);

        if (!string.IsNullOrEmpty(profileId))
            ProfileId = Guid.Parse(profileId);
    }

    public Guid Id { get; }
    public string UserName { get; } = string.Empty;
    public UserRole Role { get; }
    public Guid? ProfileId { get; }
}
