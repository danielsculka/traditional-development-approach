using ManualProg.Api.Data.Users;

namespace ManualProg.Api.Features.Auth.Services;

public interface ICurrentUser
{
    public Guid Id { get; }
    public string UserName { get; }
    public UserRole Role { get; }
    public Guid? ProfileId { get; }
}
