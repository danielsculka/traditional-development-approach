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
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .Where(post => post.Id == id)
            .Include(p => p.Likes.Where(a => a.ProfileId == currentUser.ProfileId))
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            throw new EntityNotFoundException();

        if (post.Likes.Count == 0)
            throw new InvalidOperationException("post.alreadyUnliked");

        var like = post.Likes.First();

        post.Likes.Remove(like);

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
