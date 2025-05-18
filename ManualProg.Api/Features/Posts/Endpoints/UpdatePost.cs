using ManualProg.Api.Data;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class UpdatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/{id}", HandleAsync)
        .WithSummary("Update a post");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdatePostRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .Where(post => post.Id == id && (post.IsPublic
                || post.Accesses.Any(a => a.ProfileId == currentUser.ProfileId)))
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return Results.Unauthorized();

        post.Description = request.Description;

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
