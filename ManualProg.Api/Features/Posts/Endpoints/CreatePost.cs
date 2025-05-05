using ManualProg.Api.Data;
using ManualProg.Api.Data.Posts;
using ManualProg.Api.Features.Auth.Services;
using ManualProg.Api.Features.Posts.Requests;

namespace ManualProg.Api.Features.Posts.Endpoints;

public class CreatePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/", HandleAsync)
        .WithSummary("Creates a new post");

    private static async Task<Guid> HandleAsync(CreatePostRequest request, AppDbContext db, CurrentUserService currentUser, CancellationToken cancellationToken)
    {
        if (currentUser.ProfileId == null)
            throw new UnauthorizedAccessException();

        var entity = new Post
        {
            Description = request.Description,
            ProfileId = currentUser.ProfileId.Value
        };

        _ = db.Posts.Add(entity);

        _ = await db.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
