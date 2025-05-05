namespace ManualProg.Api.Features;

public interface IEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}
