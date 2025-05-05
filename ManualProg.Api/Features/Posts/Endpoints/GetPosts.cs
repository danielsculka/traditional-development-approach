using ManualProg.Api.Data;
using ManualProg.Api.Data.Extensions;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;
using ManualProg.Api.Features.Posts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class GetPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", HandleAsync)
        .WithSummary("Get posts");

    private static async Task<PagedList<PostResponse>> HandleAsync(
        GetPostsRequest request,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        var result = await db.Posts
            .Select(post => new PostResponse
            {
                Id = post.Id,
                ImageIds = post.Images
                    .Select(i => i.Id)
                    .ToArray(),
                Description = post.Description,
                CommentCount = post.Comments.Count,
                LikeCount = post.Likes.Count,
                Profile = new PostResponse.ProfileData
                {
                    Id = post.Profile.Id,
                    Name = post.Profile.Name,
                    HasImage = post.Profile.Image != null
                },
                Created = post.Created
            })
            .ToPagedArrayAsync(request, cancellationToken);

        return result;
    }
}
