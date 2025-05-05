using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.Posts;

public class PostComment : Entity<Guid>
{
    public required Guid PostId { get; set; }
    public virtual Post Post { get; set; } = null!;

    public required Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; } = null!;

    public Guid? ReplyToCommentId { get; set; }
    public virtual PostComment? ReplyToComment { get; set; }

    public virtual ICollection<PostComment> Replies { get; set; } = [];
    public virtual ICollection<PostCommentLike> Likes { get; set; } = [];
}
