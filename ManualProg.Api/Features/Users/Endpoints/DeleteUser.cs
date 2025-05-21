using ManualProg.Api.Data;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Users.Endpoints;

public class DeleteUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", HandleAsync)
        .WithSummary("Dzēst lietotāju");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var user = await db.Users
            .Where(p => p.Id == id && p.Username != "admin")
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Results.Unauthorized();

        db.Users.Remove(user);

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
