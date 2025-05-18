using ManualProg.Api.Data;
using ManualProg.Api.Data.CoinTransactions;
using ManualProg.Api.Data.Posts;
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

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCommentRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var post = await db.Posts
            .Include(p => p.Comments.Where(c => c.Id == request.ReplyToCommentId))
                .ThenInclude(c => c.Profile)
            .Where(p => p.Id == request.PostId && (
                p.IsPublic
                || p.ProfileId == currentUser.ProfileId
                || p.Accesses.Any(a => a.ProfileId == currentUser.ProfileId)))
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return Results.NotFound();

        if (request.ReplyToCommentId.HasValue && post.Comments.Count == 0)
            return Results.BadRequest("comment.replyPostMismatch");


        var comment = new PostComment
        {
            Id = Guid.NewGuid(),
            PostId = request.PostId,
            ProfileId = currentUser.ProfileId!.Value,
            ReplyToCommentId = request.ReplyToCommentId,
            Content = request.Content
        };

        if (currentUser.ProfileId != comment.ProfileId)
        {
            var currentUserProfile = await db.Profiles
                .FindAsync([currentUser.ProfileId], cancellationToken);

            comment.CoinTransaction = new CoinTransaction
            {
                Id = Guid.NewGuid(),
                SenderProfile = currentUserProfile!,
                ReceiverProfile = post.Profile,
                Amount = 4
            };

            if (request.ReplyToCommentId.HasValue)
            {
                comment.CoinTransaction.ReceiverProfile = post.Comments.First().Profile;
                comment.CoinTransaction.Amount = 2;
            }

            comment.CoinTransaction.ReceiverProfile.Coins += comment.CoinTransaction.Amount;
        }

        _ = db.PostComments.Add(comment);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(comment.Id);
    }
}
