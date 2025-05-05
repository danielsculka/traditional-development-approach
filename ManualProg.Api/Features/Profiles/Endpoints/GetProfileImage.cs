using ManualProg.Api.Data;
using ManualProg.Api.Exceptions;
using ManualProg.Api.Features.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace ManualProg.Api.Features.Profiles.Endpoints;

public class GetProfileImage : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", HandleAsync)
        .WithSummary("Get profile image");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        var imageContent = await db.Profiles
            .Where(profile => profile.Id == id)
            .Select(profile => profile.Image == null ? null : profile.Image.Content)
            .FirstOrDefaultAsync(cancellationToken);

        if (imageContent == null)
            throw new EntityNotFoundException();

        return Results.File(imageContent, MediaTypeNames.Image.Png);
    }
}
