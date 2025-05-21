using ManualProg.Api.Data;
using ManualProg.Api.Data.Extensions;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;
using ManualProg.Api.Features.Profiles.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Profiles.Endpoints;

public class GetProfileCoinTransactions : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id}/transactions", HandleAsync)
        .WithSummary("Iegūt profila monētu plūsmas");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [AsParameters] GetPostsRequest request,
        [FromServices] AppDbContext db,
        [FromServices] ICurrentUser currentUser,
        CancellationToken cancellationToken
        )
    {
        var hasFullAccess = currentUser.Role == UserRole.Administrator;

        if (!hasFullAccess && currentUser.ProfileId != id)
            return Results.Unauthorized();

        var result = await db.CoinTransactions
            .Where(c => c.SenderProfileId == id || c.ReceiverProfileId == id)
            .OrderByDescending(c => c.Created)
            .Select(ct => new ProfileCoinTransactionResponse
            {
                Id = ct.Id,
                SenderProfile = new ProfileCoinTransactionResponse.ProfileData
                {
                    Id = ct.SenderProfile.Id,
                    Name = ct.SenderProfile.Name,
                    HasImage = ct.SenderProfile.Image != null
                },
                ReceiverProfile = new ProfileCoinTransactionResponse.ProfileData
                {
                    Id = ct.ReceiverProfile.Id,
                    Name = ct.ReceiverProfile.Name,
                    HasImage = ct.ReceiverProfile.Image != null
                },
                Ammount = ct.Amount,
                Created = ct.Created
            })
            .ToPagedArrayAsync(request, cancellationToken);

        return Results.Ok(result);
    }
}
