using ManualProg.Api.Data;
using ManualProg.Api.Data.CoinTransactions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class UnlikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/unlike", HandleAsync)
        .WithSummary("Unlike a post");

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
                .ThenInclude(l => l.CoinTransaction)
                .ThenInclude(ct => ct.ReceiverProfile)
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return Results.Unauthorized();

        if (post.Likes.Count == 0)
            return Results.BadRequest("post.alreadyUnliked");

        var like = post.Likes.First();

        var transaction = like.CoinTransaction;

        if (transaction != null)
        {
            transaction.ReceiverProfile.Coins -= transaction.Amount;

            db.CoinTransactions.Remove(transaction);
        }

        post.Likes.Remove(like);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
