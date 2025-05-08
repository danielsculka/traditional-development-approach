using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.Posts;

public class PostAccess : Entity<Guid>
{
    public Guid PostId { get; set; } = Guid.Empty;
    public virtual Post Post { get; set; } = null!;

    public required Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; } = null!;
}
