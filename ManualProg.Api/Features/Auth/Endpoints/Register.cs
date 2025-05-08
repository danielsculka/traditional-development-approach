using ManualProg.Api.Data;
using ManualProg.Api.Data.Images;
using ManualProg.Api.Data.Profiles;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Requests;
using ManualProg.Api.Features.Auth.Responses;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace ManualProg.Api.Features.Auth.Endpoints;

public class Register : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/register", HandleAsync)
        .WithSummary("Register");

    private static readonly string[] AcceptedImageTypes = [MediaTypeNames.Image.Png, MediaTypeNames.Image.Jpeg];

    private static async Task<TokenResponse> HandleAsync(
        [FromBody] RegisterRequest request,
        [FromServices] AppDbContext db,
        [FromServices] IdentityService identityManager,
        CancellationToken cancellationToken
        )
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Username);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Username.Length, 3);

        ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Password);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Password.Length, 8);

        ArgumentNullException.ThrowIfNull(request.Profile);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Profile.Name);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Profile.Name.Length, 3);

        var userExists = await db.Users
            .AnyAsync(u => u.Username == request.Username, cancellationToken);

        if (userExists)
            throw new InvalidOperationException("auth.userExists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Role = UserRole.Basic,
            Profile = new Profile
            {
                Id = Guid.NewGuid(),
                Name = request.Profile.Name,
                Description = request.Profile.Description
            }
        };

        if (request.Profile.Image != null)
        {
            if (!AcceptedImageTypes.Contains(request.Profile.Image.ContentType))
                throw new InvalidOperationException("profile.imageIncorectType");

            using var ms = new MemoryStream();
            await request.Profile.Image.CopyToAsync(ms, cancellationToken);

            user.Profile.Image = new Image
            {
                Id = Guid.NewGuid(),
                Content = ms.ToArray()
            };
        }

        var hasher = new PasswordHasher<User>();

        user.Password = hasher.HashPassword(user, request.Password);

        await db.Users.AddAsync(user, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

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