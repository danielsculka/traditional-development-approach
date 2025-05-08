using ManualProg.Api.Data;
using ManualProg.Api.Data.CoinTransactions;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class PurchasePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/purchase", HandleAsync)
        .WithSummary("Purchase a post");

    private static async Task HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .Where(post => post.Id == id)
            .Include(post => post.Accesses.Where(a => a.ProfileId == currentUser.ProfileId))
            .Include(post => post.Profile)
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            throw new EntityNotFoundException();

        if (post.IsPublic || post.ProfileId == currentUser.ProfileId || post.Accesses.Count != 0)
            throw new InvalidOperationException("post.alreadyHasAccess");

        var buyerProfile = await db.Profiles
            .FindAsync([currentUser.ProfileId], cancellationToken);

        if (buyerProfile!.Coins < post.Price)
            throw new InvalidOperationException("profile.insufficientCoins");

        db.CoinTransactions.Add(new CoinTransaction(buyerProfile, post.Profile, post.Price));

        post.Accesses.Add(new PostAccess
        {
            Id = Guid.NewGuid(),
            ProfileId = currentUser.ProfileId!.Value
        });

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
