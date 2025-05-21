using ManualProg.Api.Data;
using ManualProg.Api.Data.Extensions;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;
using ManualProg.Api.Features.Posts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class GetPosts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", HandleAsync)
        .WithSummary("Iegūt ierakstus");

    private static async Task<PagedList<PostResponse>> HandleAsync(
        [AsParameters] GetPostsRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var hasFullAccess = currentUser.Role == UserRole.Administrator
            || currentUser.Role == UserRole.Moderator;

        var result = await db.Posts
            .OrderByDescending(c => c.Created)
            .Where(post => request.ProfileId == null || post.ProfileId == request.ProfileId)
            .Select(post => new PostResponse
            {
                Id = post.Id,
                ImageIds = post.Images
                    .Select(i => i.Id)
                    .ToArray(),
                Description = post.Description,
                CommentCount = post.Comments.Count,
                LikeCount = post.Likes.Count,
                HasLike = post.Likes.Any(l => l.ProfileId == currentUser.ProfileId),
                HasAccess = hasFullAccess
                    || post.IsPublic
                    || post.ProfileId == currentUser.ProfileId
                    || post.Accesses.Any(a => a.ProfileId == currentUser.ProfileId),
                Price = post.Price,
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
