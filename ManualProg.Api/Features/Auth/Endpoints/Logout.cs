using ManualProg.Api.Data;
using ManualProg.Api.Features.Auth.Requests;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Auth.Endpoints;

public class Logout : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/logout", Handle)
        .WithSummary("Izrkastīties");

    private static async Task<IResult> Handle(
        [FromBody] LogoutRequest request,
        [FromServices] AppDbContext db,
        [FromServices] IdentityService identityManager,
        CancellationToken cancellationToken
        )
    {
        var user = await db.Users
            .Where(user => user.RefreshToken == request.RefreshToken)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Results.NotFound();

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}