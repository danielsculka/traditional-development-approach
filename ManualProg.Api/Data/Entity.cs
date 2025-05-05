
namespace ManualProg.Api.Data;

public abstract class Entity<T> : IEntity<T>, IAuditable
{
    public T Id { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Modified { get; set; } = DateTime.UtcNow;
}
