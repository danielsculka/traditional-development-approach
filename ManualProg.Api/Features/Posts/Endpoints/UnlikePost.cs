using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class UnlikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/unlike", HandleAsync)
        .WithSummary("Unlike a post");

    private static async Task HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .Where(post => post.Id == id)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            throw new EntityNotFoundException();

        var like = post.Likes
            .FirstOrDefault(l => l.ProfileId == currentUser.ProfileId);

        if (like == null)
            throw new InvalidOperationException("post.alreadyUnliked");

        post.Likes.Remove(like);

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
