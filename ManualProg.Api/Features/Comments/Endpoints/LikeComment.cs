using ManualProg.Api.Data;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class LikeComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/like", HandleAsync)
        .WithSummary("Like a comment");

    private static async Task HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var comment = await db.PostComments
            .Where(p => p.Id == id)
            .Include(p => p.Likes.Where(a => a.ProfileId == currentUser.ProfileId))
            .FirstOrDefaultAsync(cancellationToken);

        if (comment == null)
            throw new EntityNotFoundException();

        if (comment.Likes.Count != 0)
            throw new InvalidOperationException("comment.alreadyLiked");

        db.PostCommentLikes.Add(new PostCommentLike
        {
            Id = Guid.NewGuid(),
            CommentId = comment.Id,
            ProfileId = currentUser.ProfileId!.Value,
        });

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
