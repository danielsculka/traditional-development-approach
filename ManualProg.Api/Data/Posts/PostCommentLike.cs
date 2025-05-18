using ManualProg.Api.Data.CoinTransactions;
using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.Posts;

public class PostCommentLike : Entity<Guid>
{
    public Guid CommentId { get; set; } = Guid.Empty;
    public virtual PostComment Comment { get; set; } = null!;

    public Guid? CoinTransactionId { get; set; }
    public virtual CoinTransaction? CoinTransaction { get; set; }

    public required Guid ProfileId { get; set; }
    public virtual Profile Profile { get; set; } = null!;
}
