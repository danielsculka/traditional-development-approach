using ManualProg.Api.Data;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Users.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Users.Endpoints;

public class UpdateUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/{id}", HandleAsync)
        .WithSummary("Atjaunot lietotāju");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        if (request.Role == UserRole.Basic)
            throw new InvalidOperationException("user.cannotCreateUserWithRoleBasic");

        var user = await db.Users
            .Where(p => p.Id == id && p.Username != "admin")
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return Results.Unauthorized();

        user.Role = request.Role;

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
