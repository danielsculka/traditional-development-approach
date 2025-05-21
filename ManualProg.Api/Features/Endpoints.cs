using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Endpoints;
using ManualProg.Api.Features.Comments.Endpoints;
using ManualProg.Api.Features.Posts.Endpoints;
using ManualProg.Api.Features.Profiles.Endpoints;
using ManualProg.Api.Features.Users.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

namespace ManualProg.Api.Features;

public static class Endpoints
{
    private static readonly OpenApiSecurityScheme securityScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Name = JwtBearerDefaults.AuthenticationScheme,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new()
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme
        }
    };

    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("")
            .WithOpenApi()
            .DisableAntiforgery();

        endpoints.MapAuthEndpoints();
        endpoints.MapPostEndpoints();
        endpoints.MapCommentEndpoints();
        endpoints.MapUserEndpoints();
        endpoints.MapProfileEndpoints();
    }

    private static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("1. Autentifikācija un autorizācija")
            .WithOrder(1);

        endpoints.MapPublicGroup()
            .MapEndpoint<Register>()
            .MapEndpoint<Login>()
            .MapEndpoint<RefreshToken>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<Logout>();
    }

    private static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/posts")
            .WithTags("4. Ieraksti")
            .WithOrder(4);

        endpoints.MapPublicGroup()
            .MapEndpoint<GetPosts>()
            .MapEndpoint<GetPost>()
            .MapEndpoint<GetPostImage>()
            .MapEndpoint<GetPostComments>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<DeletePost>();

        endpoints.MapAuthorizedGroup([UserRole.Basic])
            .MapEndpoint<CreatePost>()
            .MapEndpoint<UpdatePost>()
            .MapEndpoint<LikePost>()
            .MapEndpoint<UnlikePost>()
            .MapEndpoint<PurchasePost>();
    }

    private static void MapCommentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/comments")
            .WithTags("5. Komentāri")
            .WithOrder(5);

        endpoints.MapPublicGroup()
            .MapEndpoint<GetCommentReplies>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<DeleteComment>();

        endpoints.MapAuthorizedGroup([UserRole.Basic])
            .MapEndpoint<CreateComment>()
            .MapEndpoint<LikeComment>()
            .MapEndpoint<UnlikeComment>();
    }

    private static void MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/profiles")
            .WithTags("3. Profili")
            .WithOrder(3);

        endpoints.MapPublicGroup()
            .MapEndpoint<GetProfile>()
            .MapEndpoint<GetProfileImage>();

        endpoints.MapAuthorizedGroup()
            .MapEndpoint<GetProfileCoinTransactions>()
            .MapEndpoint<DeleteProfile>();

        endpoints.MapAuthorizedGroup([UserRole.Basic])
            .MapEndpoint<UpdateProfile>();
    }

    private static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/users")
            .WithTags("2. Lietotāji")
            .WithOrder(2);

        endpoints.MapAuthorizedGroup([UserRole.Administrator, UserRole.Moderator])
            .MapEndpoint<GetUsers>();

        endpoints.MapAuthorizedGroup([UserRole.Administrator])
            .MapEndpoint<CreateUser>()
            .MapEndpoint<UpdateUser>()
            .MapEndpoint<DeleteUser>();
    }

    private static RouteGroupBuilder MapPublicGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .AllowAnonymous();
    }

    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization()
            .WithOpenApi(x => new(x)
            {
                Security = [new() { [securityScheme] = [] }],
            });
    }

    private static RouteGroupBuilder MapAuthorizedGroup(this IEndpointRouteBuilder app, IEnumerable<UserRole> roles, string? prefix = null)
    {
        return app.MapGroup(prefix ?? string.Empty)
            .RequireAuthorization(new AuthorizeAttribute { Roles = string.Join(',', roles) })
            .WithOpenApi(x => new(x)
            {
                Security = [new() { [securityScheme] = [] }],
            });
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
