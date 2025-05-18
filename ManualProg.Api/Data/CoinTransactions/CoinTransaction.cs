using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.CoinTransactions;

public class CoinTransaction : Entity<Guid>
{
    public Guid SenderProfileId { get; init; }
    public required virtual Profile SenderProfile { get; init; }

    public Guid ReceiverProfileId { get; init; }
    public required virtual Profile ReceiverProfile { get; set; }

    public required int Amount { get; set; }
}
