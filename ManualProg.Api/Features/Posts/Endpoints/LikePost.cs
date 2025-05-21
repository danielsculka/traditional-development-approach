using ManualProg.Api.Data;
using ManualProg.Api.Data.CoinTransactions;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class LikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/like", HandleAsync)
        .WithSummary("Atzīmēt ierakstu ar `Patīk`");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .Where(post => post.Id == id && (
                post.IsPublic
                || post.ProfileId == currentUser.ProfileId
                || post.Accesses.Any(a => a.ProfileId == currentUser.ProfileId)))
            .Include(p => p.Likes.Where(a => a.ProfileId == currentUser.ProfileId))
            .Include(post => post.Profile)
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return Results.Unauthorized();

        if (post.Likes.Count != 0)
            return Results.BadRequest("post.alreadyLiked");

        var like = new PostLike
        {
            Id = Guid.NewGuid(),
            PostId = post.Id,
            ProfileId = currentUser.ProfileId!.Value
        };

        if (currentUser.ProfileId != post.ProfileId)
        {
            var currentUserProfile = await db.Profiles
                .FindAsync([currentUser.ProfileId], cancellationToken);

            like.CoinTransaction = new CoinTransaction
            {
                Id = Guid.NewGuid(),
                SenderProfile = currentUserProfile!,
                ReceiverProfile = post.Profile,
                Amount = 2
            };

            like.CoinTransaction.ReceiverProfile.Coins += like.CoinTransaction.Amount;
        }

        db.PostLikes.Add(like);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
