using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.Posts;

public class PostLike : Entity<Guid>
{
    public required Guid PostId { get; set; }
    public virtual Post Post { get; set; } = null!;

    public required Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; } = null!;
}
