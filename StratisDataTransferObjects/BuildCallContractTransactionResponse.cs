namespace Logicing.Knowing.StratisDataTransferObjects;

public partial class BuildCallContractTransactionResponse
{
    public Money Fee { get; set; }
    public string Hex { get; set; }
    public string Message { get; set; }
    public bool? Success { get; set; }
    public string TransactionId { get; set; }
}
