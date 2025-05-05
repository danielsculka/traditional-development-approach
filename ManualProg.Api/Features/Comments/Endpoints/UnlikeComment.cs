using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class UnlikeComment : IEndpoint
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
        var comment = await db.PostComments
            .Where(p => p.Id == id)
            .Include(p => p.Likes)
            .FirstOrDefaultAsync(cancellationToken);

        if (comment == null)
            throw new EntityNotFoundException();

        var like = comment.Likes
            .FirstOrDefault(l => l.ProfileId == currentUser.ProfileId);

        if (like == null)
            throw new InvalidOperationException("comment.alreadyUnliked");

        comment.Likes.Remove(like);

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
