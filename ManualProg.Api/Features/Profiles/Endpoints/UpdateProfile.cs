using ManualProg.Api.Data;
using ManualProg.Api.Data.Images;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Profiles.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ManualProg.Api.Features.Profiles.Endpoints;

public class UpdateProfile : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPut("/{id}", HandleAsync)
        .WithSummary("Atjaunot profilu");

    private static readonly string[] AcceptedImageTypes = [MediaTypeNames.Image.Png, MediaTypeNames.Image.Jpeg];

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProfileRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        if (currentUser.ProfileId != id)
            return Results.Unauthorized();

        ArgumentNullException.ThrowIfNullOrWhiteSpace(request.Name);
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Name.Length, 3);

        var profile = await db.Profiles
            .FindAsync([id], cancellationToken);

        if (profile == null)
            return Results.NotFound();

        profile.Name = request.Name;
        profile.Description = request.Description;

        if (request.Image != null)
        {
            if (!AcceptedImageTypes.Contains(request.Image.ContentType))
                throw new InvalidOperationException("profile.imageIncorectType");

            using var ms = new MemoryStream();
            await request.Image.CopyToAsync(ms, cancellationToken);

            var imageContent = ms.ToArray();

            if (profile.Image == null)
            {
                profile.Image = new Image
                {
                    Content = imageContent
                };
            } 
            else
            {
                profile.Image.Content = imageContent;
            }
        }

        _ = await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}
