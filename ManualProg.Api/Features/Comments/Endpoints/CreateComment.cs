using ManualProg.Api.Data;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Comments.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class CreateComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", HandleAsync)
        .WithSummary("Creates a new comment");

    private static async Task<Guid> HandleAsync(
        [FromBody] CreateCommentRequest request, 
        [FromServices] AppDbContext db, 
        [FromServices] CurrentUserService currentUser, 
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .Include(p => p.Comments.Where(c => c.Id == request.ReplyToCommentId))
            .Where(p => p.Id == request.PostId)
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            throw new EntityNotFoundException();

        if (request.ReplyToCommentId.HasValue && post.Comments.Count == 0)
            throw new InvalidOperationException("comment.replyPostMismatch");

        var comment = new PostComment
        {
            PostId = request.PostId,
            ProfileId = currentUser.ProfileId!.Value,
            ReplyToCommentId = request.ReplyToCommentId,
            Content = request.Content
        };

        _ = db.PostComments.Add(comment);

        _ = await db.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
