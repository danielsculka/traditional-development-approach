using ManualProg.Api.Data;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class DeletePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", HandleAsync)
        .WithSummary("Dzēst ierakstu");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var hasFullAccess = currentUser.Role == UserRole.Administrator
            || currentUser.Role == UserRole.Moderator;

        var post = await db.Posts
            .Include(p => p.Comments)
            .Where(p => p.Id == id && (hasFullAccess || p.ProfileId == currentUser.ProfileId))
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return Results.Unauthorized();

        foreach (var reply in post.Comments)
        {
            DeleteComment(db, reply);
        }


        db.Posts.Remove(post);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    private static void DeleteComment(AppDbContext db, PostComment comment)
    {
        foreach (var reply in comment.Replies)
        {
            DeleteComment(db, reply);
        }

        var transaction = comment.CoinTransaction;

        transaction.ReceiverProfile.Coins -= transaction.Amount;

        db.CoinTransactions.Remove(transaction);

        db.PostComments.Remove(comment);
    }
}
