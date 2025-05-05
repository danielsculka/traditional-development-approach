namespace ManualProg.Api.Data;

public interface IAuditable
{
    DateTime Created { get; set; }
    DateTime Modified { get; set; }
}
