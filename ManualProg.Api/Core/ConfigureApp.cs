using ManualProg.Api.Data;
using ManualProg.Api.Features;
using ManualProg.Api.Features.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace ManualProg.Api.Core;

public static class ConfigureApp
{
    public static async Task Configure(this WebApplication app)
    {
        await app.EnsureDatabaseAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseCors("policy");

        app.MapEndpoints();
    }

    private static async Task EnsureDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var dbCreator = db.GetService<IRelationalDatabaseCreator>();

        var creationStrategy = db.Database.CreateExecutionStrategy();
        await creationStrategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync())
                await dbCreator.CreateAsync();
        });

        await AppDbContextSetup.MigrateAsync(db);

        var adminOptions = scope.ServiceProvider.GetRequiredService<IOptions<AdminOptions>>();

        await AppDbContextSetup.SeedAsync(db, adminOptions.Value);
    }
}
