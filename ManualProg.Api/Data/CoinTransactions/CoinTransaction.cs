using ManualProg.Api.Data.Profiles;

namespace ManualProg.Api.Data.CoinTransactions;

public class CoinTransaction : Entity<Guid>
{
    public CoinTransaction(Profile sender, Profile receiver, int amount)
    {
        Id = Guid.NewGuid();
        SenderProfile = sender;
        ReceiverProfile = receiver;
        Amount = amount;

        SenderProfile.Coins -= amount;
        ReceiverProfile.Coins += amount;
    }

    public Guid SenderProfileId { get; init; }
    public virtual Profile SenderProfile { get; init; }

    public Guid ReceiverProfileId { get; init; }
    public virtual Profile ReceiverProfile { get; init; }

    public int Amount { get; init; }
}
