using ManualProg.Api.Data.Posts;
using ManualProg.Api.Data.Profiles;
using ManualProg.Api.Data.Users;
using ManualProg.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ManualProg.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConvertEnumToString(modelBuilder);

        _ = modelBuilder.Entity<User>(entity =>
        {
            entity.HasOne(c => c.Profile)
                .WithOne(c => c.User)
                .HasForeignKey<Profile>(c => c.UserId);
        });

        _ = modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasOne(c => c.User)
                .WithOne(c => c.Profile)
                .HasForeignKey<User>(c => c.ProfileId);

            entity.HasMany(c => c.TransactionsAsSender)
                .WithOne(c => c.SenderProfile)
                .HasForeignKey(c => c.SenderProfileId);

            entity.HasMany(c => c.TransactionsAsReciever)
                .WithOne(c => c.ReceiverProfile)
                .HasForeignKey(c => c.ReceiverProfileId);
        });

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        PrepareEntries();

        int result;

        try
        {
            result = await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new DatabaseException(DatabaseException.DefaultMessage, ex);
        }

        return result;
    }

    private static void ConvertEnumToString(ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            foreach (IMutableProperty property in entityType.GetProperties())
            {
                Type nullableType = Nullable.GetUnderlyingType(property.ClrType)!;

                if (property.ClrType.IsEnum || nullableType?.IsEnum == true)
                {
                    Type type = typeof(EnumToStringConverter<>).MakeGenericType(nullableType ?? property.ClrType);
                    var converter = Activator.CreateInstance(type, new ConverterMappingHints()) as ValueConverter;

                    property.SetValueConverter(converter);
                }
            }
    }

    private void PrepareEntries()
    {
        DateTime now = DateTime.UtcNow;

        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Entity<Guid>)
            {
                var guidEntry = entry.Entity as Entity<Guid>;

                if (guidEntry!.Id == Guid.Empty)
                    throw new Exception("Empty GUIDs are not allowed.");
            }

            if (entry.Entity is IAuditable)
            {
                var auditableEntry = entry.Entity as IAuditable;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntry.Created = now;
                        auditableEntry.Modified = now;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        auditableEntry.Modified = now;
                        break;
                }
            }
        }
    }
}
