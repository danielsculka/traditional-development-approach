using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Requests;
using ManualProg.Api.Features.Auth.Responses;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Auth.Endpoints;

public class RefreshToken : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/refresh", Handle)
        .WithSummary("Refresh token");

    private static async Task<TokenResponse> Handle(
        [FromBody] RefreshTokenRequest request,
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

        if (user.RefreshToken != request.RefreshToken
            || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new AccessDeniedException();

        var refreshToken = identityManager.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await db.SaveChangesAsync(cancellationToken);

        return new TokenResponse
        {
            AccessToken = identityManager.GenerateToken(user),
            RefreshToken = user.RefreshToken,
            UserId = user.Id,
            Username = user.Username,
            UserRole = user.Role,
            ProfileId = user.ProfileId
        };
    }
}