using ManualProg.Api.Data;
using ManualProg.Api.Data.Extensions;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Comments.Requests;
using ManualProg.Api.Features.Comments.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class GetCommentReplies : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/replies", HandleAsync)
        .WithSummary("Gets all replies to a comment");

    private static async Task<PagedList<CommentResponse>> HandleAsync(
        [FromRoute] Guid id,
        [AsParameters] GetCommentsRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var result = await db.PostComments
            .Where(c => c.ReplyToCommentId == id)
            .OrderByDescending(c => c.Created)
            .Select(c => new CommentResponse
            {
                Id = c.Id,
                Content = c.Content,
                RepliesCount = c.Replies.Count,
                LikeCount = c.Likes.Count,
                HasLike = c.Likes.Any(l => l.ProfileId == currentUser.ProfileId),
                Profile = new CommentResponse.ProfileData
                {
                    Id = c.Profile.Id,
                    Name = c.Profile.Name,
                    HasImage = c.Profile.Image != null
                },
                Created = c.Created
            })
            .ToPagedArrayAsync(request, cancellationToken);

        return result;
    }
}
