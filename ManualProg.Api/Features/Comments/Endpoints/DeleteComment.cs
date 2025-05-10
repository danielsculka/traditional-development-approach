using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Comments.Endpoints;

public class DeleteComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", HandleAsync)
        .WithSummary("Delete a comment");

    private static async Task HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        //TODO: rework comment deletion / should delete all replies in recursion

        var comment = await db.PostComments
            .FindAsync([id], cancellationToken);

        if (comment == null)
            throw new EntityNotFoundException();

        db.PostComments.Remove(comment);

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
