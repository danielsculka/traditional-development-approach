using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Requests;
using ManualProg.Api.Features.Auth.Responses;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Auth.Endpoints;

public class Login : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/login", HandleAsync)
        .WithSummary("Login");

    private static async Task<TokenResponse> HandleAsync(
        [FromBody] LoginRequest request,
        [FromServices] AppDbContext db,
        [FromServices] IdentityService identityManager,
        CancellationToken cancellationToken
        )
    {
        var user = await db.Users
            .Where(u => u.Username == request.Username)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new EntityNotFoundException();

        if (!identityManager.VerifyPassword(user, request.Password))
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
            UserName = user.Username,
            UserRole = user.Role
        };
    }
}