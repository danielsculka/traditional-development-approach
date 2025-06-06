﻿using ManualProg.Api.Data;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class GetPost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}", HandleAsync)
        .WithSummary("Iegūt ierakstu");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id, 
        [FromServices] AppDbContext db, 
        [FromServices] ICurrentUser currentUser, 
        CancellationToken cancellationToken
        )
    {
        var hasFullAccess = currentUser.Role == UserRole.Administrator
            || currentUser.Role == UserRole.Moderator;

        var post = await db.Posts
            .Where(post => post.Id == id)
            .Select(post => new PostResponse
            {
                Id = post.Id,
                ImageIds = post.Images
                    .Select(i => i.Id)
                    .ToArray(),
                Description = post.Description,
                CommentCount = post.Comments.Count,
                LikeCount = post.Likes.Count,
                HasLike = post.Likes.Any(l => l.ProfileId == currentUser.ProfileId),
                HasAccess = hasFullAccess
                    || post.IsPublic
                    || post.ProfileId == currentUser.ProfileId
                    || post.Accesses.Any(a => a.ProfileId == currentUser.ProfileId),
                Price = post.Price,
                Profile = new PostResponse.ProfileData
                {
                    Id = post.Profile.Id,
                    Name = post.Profile.Name,
                    HasImage = post.Profile.Image != null
                },
                Created = post.Created
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (post == null)
            return Results.NotFound();

        return Results.Ok(post);
    }
}
