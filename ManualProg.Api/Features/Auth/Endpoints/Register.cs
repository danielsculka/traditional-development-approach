using ManualProg.Api.Data;
using ManualProg.Api.Data.Profiles;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Requests;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ManualProg.Api.Features.Auth.Endpoints;

public class Register : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/register", HandleAsync)
        .WithSummary("Register");

    private static async Task HandleAsync(
        RegisterRequest request,
        AppDbContext db,
        IdentityService identityManager,
        CancellationToken cancellationToken
        )
    {
        if (string.IsNullOrEmpty(request.Username) || request.Password.Length < 3
            || request.Profile == null || string.IsNullOrEmpty(request.Profile.Name) || request.Password.Length < 3
            || string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
            throw new ValidationException();

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
                Name = request.Profile.Name,
                Description = request.Profile.Description
            }
        };

        var hasher = new PasswordHasher<User>();

        user.Password = hasher.HashPassword(user, request.Password);

        await db.Users.AddAsync(user, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }
}