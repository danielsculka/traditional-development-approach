using ManualProg.Api.Data.Users;
using ManualProg.Api.Features.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManualProg.Api.Data;

public static class AppDbContextSetup
{
    public static async Task MigrateAsync(AppDbContext db)
    {
        var strategy = db.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await db.Database.BeginTransactionAsync();
            await db.Database.MigrateAsync();
            await transaction.CommitAsync();
        });
    }

    public static async Task SeedAsync(AppDbContext db, AdminOptions options)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(user => user.Username == options.Username);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Username = options.Username
            };

            _ = db.Users.Add(user);
        }

        user.Role = UserRole.Administrator;

        var hasher = new PasswordHasher<User>();

        user.Password = hasher.HashPassword(user, options.Password);

        _ = await db.SaveChangesAsync();
    }
}
