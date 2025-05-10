using ManualProg.Api.Data;
using ManualProg.Api.Data.Extensions;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;
using ManualProg.Api.Features.Profiles.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Profiles.Endpoints;

public class GetProfilePosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/posts", HandleAsync)
        .WithSummary("Get a profile posts");

    private static async Task<PagedList<ProfilePostResponse>> HandleAsync(
        [FromRoute] Guid id,
        [AsParameters] GetPostsRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var result = await db.Posts
            .Where(p => p.ProfileId == id)
            .OrderByDescending(c => c.Created)
            .Select(post => new ProfilePostResponse
            {
                Id = post.Id,
                ImageIds = post.Images
                    .Select(i => i.Id)
                    .ToArray(),
                Description = post.Description,
                CommentCount = post.Comments.Count,
                LikeCount = post.Likes.Count,
                Created = post.Created
            })
            .ToPagedArrayAsync(request, cancellationToken);

        return result;
    }
}
