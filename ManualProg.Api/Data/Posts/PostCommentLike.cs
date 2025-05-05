using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.Posts;

public class PostCommentLike : Entity<Guid>
{
    public Guid CommentId { get; set; } = Guid.Empty;
    public virtual PostComment Comment { get; set; } = null!;

    public required Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; } = null!;
}
