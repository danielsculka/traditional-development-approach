﻿using ManualProg.Api.Data;
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
        .WithSummary("Pierakstīties");

    private static async Task<IResult> HandleAsync(
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
            return Results.NotFound();

        if (!identityManager.VerifyPassword(user, request.Password))
            return Results.Unauthorized();

        var refreshToken = identityManager.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(new TokenResponse
        {
            AccessToken = identityManager.GenerateToken(user),
            RefreshToken = user.RefreshToken,
            UserId = user.Id,
            Username = user.Username,
            UserRole = user.Role,
            ProfileId = user.ProfileId
        });
    }
}