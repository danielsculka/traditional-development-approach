using ManualProg.Api.Data;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Users.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Users.Endpoints;

public class CreateUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", HandleAsync)
        .WithSummary("Creates a new user");

    private static async Task<Guid> HandleAsync(
        [FromForm] CreateUserRequest request, 
        [FromServices] AppDbContext db, 
        [FromServices] ICurrentUser currentUser, 
        CancellationToken cancellationToken
        )
    {
        if (request.Role == UserRole.Basic)
            throw new InvalidOperationException("user.cannotCreateUserWithRoleBasic");

        ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Username);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Username.Length, 3);

        ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Password);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Password.Length, 8);

        var userExists = await db.Users
            .AnyAsync(u => u.Username == request.Username, cancellationToken);

        if (userExists)
            throw new InvalidOperationException("user.userExists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Role = request.Role
        };

        var hasher = new PasswordHasher<User>();

        user.Password = hasher.HashPassword(user, request.Password);

        await db.Users.AddAsync(user, cancellationToken);

        _ = await db.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
