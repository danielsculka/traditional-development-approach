using ManualProg.Api.Data.Images;

namespace ManualProg.Api.Data.Posts;

public class PostImage : Entity<Guid>
{
    public Guid PostId { get; set; } = Guid.Empty;
    public virtual Post Post { get; set; } = null!;

    public Guid ImageId { get; set; } = Guid.Empty;
    public virtual Image Image { get; set; } = null!;
}
