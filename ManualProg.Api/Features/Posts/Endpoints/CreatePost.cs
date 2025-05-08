using ManualProg.Api.Data;
using ManualProg.Api.Data.Images;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class CreatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", HandleAsync)
        .WithSummary("Creates a new post");

    private static readonly string[] AcceptedImageTypes = [MediaTypeNames.Image.Png, MediaTypeNames.Image.Jpeg];

    private static async Task<Guid> HandleAsync(
        [FromForm] CreatePostRequest request, 
        [FromServices] AppDbContext db, 
        [FromServices] CurrentUserService currentUser, 
        CancellationToken cancellationToken
        )
    {
        if (!request.Images.Any())
            throw new InvalidOperationException("post.imageRequired");

        var images = new List<PostImage>();

        foreach (var image in request.Images)
        {
            if (!AcceptedImageTypes.Contains(image.ContentType))
                throw new InvalidOperationException("post.imageIncorectType");

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms, cancellationToken);

            images.Add(new PostImage
            {
                Image = new Image
                {
                    Id = Guid.NewGuid(),
                    Content = ms.ToArray()
                }
            });
        }

        var post = new Post
        {
            Id = Guid.NewGuid(),
            IsPublic = request.IsPublic,
            Description = request.Description,
            ProfileId = currentUser.ProfileId!.Value,
            Images = images
        };

        _ = db.Posts.Add(post);

        _ = await db.SaveChangesAsync(cancellationToken);

        return post.Id;
    }
}
