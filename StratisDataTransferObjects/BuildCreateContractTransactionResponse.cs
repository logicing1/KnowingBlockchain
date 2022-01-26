namespace Logicing.Knowing.StratisDataTransferObjects;

public class BuildCreateContractTransactionResponse
{
    public string NewContractAddress { get; set; }
    public Money Fee { get; set; }
    public string Hex { get; set; }
    public string Message { get; set; }
    public bool? Success { get; set; }
    public string TransactionId { get; set; }

}