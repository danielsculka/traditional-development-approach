﻿using ManualProg.Api.Data;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Profiles.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Profiles.Endpoints;

public class GetProfile : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", HandleAsync)
        .WithSummary("Iegūt profilu");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id, 
        [FromServices] AppDbContext db, 
        [FromServices] ICurrentUser currentUser, 
        CancellationToken cancellationToken
        )
    {
        var profile = await db.Profiles
            .Where(p => p.Id == id)
            .Select(p => new ProfileResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                HasImage = p.Image != null,
                PostCount = p.Posts.Count
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
            return Results.NotFound();

        return Results.Ok(profile);
    }
}
