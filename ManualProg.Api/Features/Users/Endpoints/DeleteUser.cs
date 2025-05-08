using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Users.Endpoints;

public class DeleteUser : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}", HandleAsync)
        .WithSummary("Delete a user");

    private static async Task HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        var user = await db.Users
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            throw new EntityNotFoundException();

        db.Users.Remove(user);

        _ = await db.SaveChangesAsync(cancellationToken);
    }
}
