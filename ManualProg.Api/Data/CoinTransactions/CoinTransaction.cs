using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.CoinTransactions;

public class CoinTransaction : Entity<Guid>
{
    public required Guid SenderProfileId { get; set; }
    public virtual Profile SenderProfile { get; set; } = null!;

    public required Guid ReceiverProfileId { get; set; }
    public virtual Profile ReceiverProfile { get; set; } = null!;

    public required int Amount { get; set; }
}
