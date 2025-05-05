namespace ManualProg.Api.Data.Images;

public class Image : Entity<Guid>
{
    public required byte[] Content { get; set; }
}
