using ManualProg.Api.Data.CoinTransactions;
using ManualProg.Api.Data.Images;
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
    public DbSet<CoinTransaction> CoinTransactions { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<PostAccess> PostAccess { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<PostComment> PostComments { get; set; }
    public DbSet<PostCommentLike> PostCommentLikes { get; set; }
    public DbSet<Image> Images { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConvertEnumToString(modelBuilder);

        _ = modelBuilder.Entity<User>(entity =>
        {
            entity.HasOne(c => c.Profile)
                .WithOne(c => c.User)
                .HasForeignKey<Profile>(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        _ = modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasOne(c => c.User)
                .WithOne(c => c.Profile)
                .HasForeignKey<User>(c => c.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Image)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        });

        _ = modelBuilder.Entity<CoinTransaction>(entity =>
        {
            entity.HasOne(c => c.SenderProfile)
                .WithMany()
                .HasForeignKey(c => c.SenderProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.ReceiverProfile)
                .WithMany()
                .HasForeignKey(c => c.ReceiverProfileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        _ = modelBuilder.Entity<Post>(entity =>
        {
            entity.HasOne(c => c.Profile)
                .WithMany(c => c.Posts)
                .HasForeignKey(c => c.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Likes)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Images)
                .WithOne(c => c.Post)
                .OnDelete(DeleteBehavior.Cascade);
        });

        _ = modelBuilder.Entity<PostImage>(entity =>
        {
            entity.ToTable("PostImages");

            entity.HasOne(c => c.Image)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        });

        _ = modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasOne(c => c.Profile)
                .WithMany(c => c.PostLikes)
                .HasForeignKey(c => c.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.CoinTransaction)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
        });

        _ = modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasMany(c => c.Likes)
                .WithOne(c => c.Comment)
                .HasForeignKey(c => c.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Profile)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.ReplyToComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ReplyToCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.CoinTransaction)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
        });

        _ = modelBuilder.Entity<PostCommentLike>(entity =>
        {
            entity.HasOne(c => c.Profile)
                .WithMany(c => c.CommentLikes)
                .HasForeignKey(c => c.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.CoinTransaction)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
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
