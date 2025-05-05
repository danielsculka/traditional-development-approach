using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class UpdatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/{id}", HandleAsync)
        .WithSummary("Delete a post");

    private static async Task HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdatePostRequest request,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .FindAsync([id], cancellationToken);

        if (post == null)
            throw new EntityNotFoundException();

        post.Description = request.Description;

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
