using ManualProg.Api.Data;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class LikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{id}/like", HandleAsync)
        .WithSummary("Like a post");

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

        if (post.Likes.Any(l => l.ProfileId == currentUser.ProfileId))
            throw new InvalidOperationException("post.alreadyLiked");

        post.Likes.Add(new PostLike
        {
            ProfileId = currentUser.ProfileId!.Value,
        });

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
