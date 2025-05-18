using ManualProg.Api.Data;
using ManualProg.Api.Data.CoinTransactions;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class LikeComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/like", HandleAsync)
        .WithSummary("Like a comment");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var comment = await db.PostComments
            .Where(c => c.Id == id && (
                c.Post.IsPublic
                || c.Post.ProfileId == currentUser.ProfileId
                || c.Post.Accesses.Any(a => a.ProfileId == currentUser.ProfileId)))
            .Include(p => p.Likes.Where(a => a.ProfileId == currentUser.ProfileId))
            .Include(post => post.Profile)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment == null)
            return Results.NotFound();

        if (comment.Likes.Count != 0)
            return Results.BadRequest("comment.alreadyLiked");

        var like = new PostCommentLike
        {
            Id = Guid.NewGuid(),
            CommentId = comment.Id,
            ProfileId = currentUser.ProfileId!.Value
        };

        if (currentUser.ProfileId != comment.ProfileId)
        {
            var currentUserProfile = await db.Profiles
                .FindAsync([currentUser.ProfileId], cancellationToken);

            like.CoinTransaction = new CoinTransaction
            {
                Id = Guid.NewGuid(),
                SenderProfile = currentUserProfile!,
                ReceiverProfile = comment.Profile,
                Amount = 1
            };

            like.CoinTransaction.ReceiverProfile.Coins += like.CoinTransaction.Amount;
        }

        db.PostCommentLikes.Add(like);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
