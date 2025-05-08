namespace ManualProg.Api.Features.Profiles.Responses;

public record ProfileCoinTransactionResponse
{
    public required Guid Id { get; set; }
    public required ProfileData SenderProfile { get; set; }
    public required ProfileData ReceiverProfile { get; set; }
    public required int Ammount { get; set; }
    public required DateTime Created { get; set; }

    public record ProfileData
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required bool HasImage { get; set; }
    }
}
