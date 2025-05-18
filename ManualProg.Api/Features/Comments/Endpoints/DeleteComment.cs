using ManualProg.Api.Data;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class DeleteComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", HandleAsync)
        .WithSummary("Delete a comment");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var hasFullAccess = currentUser.Role == UserRole.Administrator
            || currentUser.Role == UserRole.Moderator;

        var comment = await db.PostComments
            .Where(c => c.Id == id && (
                hasFullAccess
                || c.ProfileId == currentUser.ProfileId))
            .FirstOrDefaultAsync(cancellationToken);

        if (comment == null)
            return Results.Unauthorized();

        Delete(db, comment);

        db.PostComments.Remove(comment);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    private static void Delete(AppDbContext db, PostComment comment)
    {
        foreach (var reply in comment.Replies)
        {
            Delete(db, reply);
        }

        var transaction = comment.CoinTransaction;

        if (transaction != null)
        {
            transaction.ReceiverProfile.Coins -= transaction.Amount;

            db.CoinTransactions.Remove(transaction);
        }

        db.PostComments.Remove(comment);
    }
}
