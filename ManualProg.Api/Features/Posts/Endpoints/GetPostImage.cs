using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
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
        var post = await db.Posts
            .Where(post => post.Images.Any(i => i.Id == id))
            .Select(post => new
            {
                ImageContent = post.Images
                    .First(i => i.Id == id)
                    .Image.Content
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            throw new EntityNotFoundException();

        return Results.File(post.ImageContent, MediaTypeNames.Image.Png);
    }
}
