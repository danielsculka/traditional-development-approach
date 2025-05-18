using ManualProg.Api.Data;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class UnlikeComment : IEndpoint
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
        var comment = await db.PostComments
            .Where(c => c.Id == id && (
                c.Post.IsPublic
                || c.Post.ProfileId == currentUser.ProfileId
                || c.Post.Accesses.Any(a => a.ProfileId == currentUser.ProfileId)))
            .Include(p => p.Likes.Where(a => a.ProfileId == currentUser.ProfileId))
                .ThenInclude(l => l.CoinTransaction)
                .ThenInclude(ct => ct.ReceiverProfile)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment == null)
            return Results.NotFound();

        if (comment.Likes.Count == 0)
            return Results.BadRequest("comment.alreadyUnliked");

        var like = comment.Likes.First();

        var transaction = like.CoinTransaction;

        if (transaction != null)
        {
            transaction.ReceiverProfile.Coins -= transaction.Amount;

            db.CoinTransactions.Remove(transaction);
        }

        comment.Likes.Remove(like);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
