﻿using ManualProg.Api.Data.Images;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Data.Users;

namespace ManualProg.Api.Data.Profiles;

public class Profile : Entity<Guid>
{
    public required string Name { get; set; } 
    public string Description { get; set; } = string.Empty;

    public int Coins { get; set; } = 0;

    public Guid? ImageId { get; set; }
    public virtual Image? Image { get; set; }

    public Guid UserId { get; set; } = Guid.Empty;
    public virtual User User { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = [];
    public virtual ICollection<PostLike> PostLikes { get; set; } = [];
    public virtual ICollection<PostComment> Comments { get; set; } = [];
    public virtual ICollection<PostCommentLike> CommentLikes { get; set; } = [];
}
