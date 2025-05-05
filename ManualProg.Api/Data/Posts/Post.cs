using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.Posts;

public class Post : Entity<Guid>
{
    public required string Description { get; set; }

    public required Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; } = null!;

    public virtual ICollection<PostComment> Comments { get; set; } = [];
    public virtual ICollection<PostLike> Likes { get; set; } = [];
    public virtual ICollection<PostImage> Images { get; set; } = [];
}
