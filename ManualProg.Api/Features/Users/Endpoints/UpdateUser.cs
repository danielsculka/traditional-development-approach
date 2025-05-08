using ManualProg.Api.Data;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Users.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Users.Endpoints;

public class UpdateUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/{id}", HandleAsync)
        .WithSummary("Update a post");

    private static async Task HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        if (request.Role == UserRole.Basic)
            throw new InvalidOperationException("user.cannotCreateUserWithRoleBasic");

        var user = await db.Users
            .FindAsync([id], cancellationToken);

        if (user == null)
            throw new EntityNotFoundException();

        user.Role = request.Role;

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
