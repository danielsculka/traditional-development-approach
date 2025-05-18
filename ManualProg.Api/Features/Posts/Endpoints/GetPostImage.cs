using ManualProg.Api.Data;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class GetPostImage : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/images/{id}", HandleAsync)
        .WithSummary("Get a post image");

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
            .Where(post => post.Images.Any(i => i.Id == id))
            .Select(post => new
            {
                HasAccess = hasFullAccess
                    || post.IsPublic
                    || post.ProfileId == currentUser.ProfileId
                    || post.Accesses.Any(a => a.ProfileId == currentUser.ProfileId),
                ImageContent = post.Images
                    .First(i => i.Id == id)
                    .Image.Content
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return Results.NotFound();

        if (!post.HasAccess)
            return Results.Unauthorized();

        return Results.File(post.ImageContent, MediaTypeNames.Image.Png);
    }
}
