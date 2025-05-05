using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Requests;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Auth.Endpoints;

public class Logout : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/logout", Handle)
        .WithSummary("Logout");

    private static async Task Handle(
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
            throw new EntityNotFoundException();

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        await db.SaveChangesAsync(cancellationToken);
    }
}