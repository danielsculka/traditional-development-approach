using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth.Endpoints;
using ManualProg.Api.Features.Posts.Endpoints;
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
            //.AddEndpointFilter<RequestLoggingFilter>()
            .WithOpenApi();

        endpoints.MapAuthEndpoints();
        endpoints.MapPostEndpoints();
    }

    private static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/auth")
            .WithTags("Auth");

        endpoints.MapPublicGroup()
            .MapEndpoint<Register>()
            .MapEndpoint<Login>();
    }

    private static void MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/posts")
            .WithTags("Posts");

        //endpoints.MapPublicGroup()
        //    .MapEndpoint<GetPosts>()
        //    .MapEndpoint<GetPostById>()
        //    .MapEndpoint<GetPostComments>();

        endpoints.MapAuthorizedGroup([UserRole.Basic])
            .MapEndpoint<CreatePost>();
        //.MapEndpoint<UpdatePost>()
        //.MapEndpoint<DeletePost>()
        //.MapEndpoint<LikePost>()
        //.MapEndpoint<UnlikePost>();
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
