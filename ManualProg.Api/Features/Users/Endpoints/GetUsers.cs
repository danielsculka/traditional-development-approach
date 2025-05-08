using ManualProg.Api.Data;
using ManualProg.Api.Data.Extensions;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Responses;
using ManualProg.Api.Features.Users.Requests;
using ManualProg.Api.Features.Users.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ManualProg.Api.Features.Users.Endpoints;

public class GetUsers : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/", HandleAsync)
        .WithSummary("Get users");

    private static async Task<PagedList<UserResponse>> HandleAsync(
        [AsParameters] GetUsersRequest request,
        [FromServices] AppDbContext db,
        [FromServices] CurrentUserService currentUser,
        CancellationToken cancellationToken
        )
    {
        var result = await db.Users
            .OrderByDescending(u => u.Created)
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                Profile = u.Profile == null 
                    ? null 
                    : new UserResponse.ProfileData
                    {
                        Id = u.Profile.Id,
                        Name = u.Profile.Name,
                    },
                Created = u.Created
            })
            .ToPagedArrayAsync(request, cancellationToken);

        return result;
    }
}
