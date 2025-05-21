using ManualProg.Api.Data;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Profiles.Endpoints;

public class DeleteProfile : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", HandleAsync)
        .WithSummary("Dzēst profilu");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var hasFullAccess = currentUser.Role == UserRole.Administrator;

        if (!hasFullAccess && currentUser.ProfileId != id)
            return Results.Unauthorized();

        var profile = await db.Profiles
            .Include(p => p.Image)
            .Include(p => p.Posts)
            .Include(p => p.PostLikes)
            .Include(p => p.Comments)
            .Include(p => p.CommentLikes)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
            return Results.NotFound();

        db.PostCommentLikes.RemoveRange(profile.CommentLikes);

        db.PostComments.RemoveRange(profile.Comments);

        db.PostLikes.RemoveRange(profile.PostLikes);

        db.Posts.RemoveRange(profile.Posts);

        if (profile.Image != null)
            db.Images.Remove(profile.Image);

        db.Profiles.Remove(profile);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
