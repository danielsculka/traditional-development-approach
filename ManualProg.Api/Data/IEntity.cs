namespace ManualProg.Api.Data;

public interface IEntity<T>
{
    T Id { get; set; }
}
