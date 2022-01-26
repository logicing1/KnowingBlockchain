namespace Logicing.Knowing.StratisDataTransferObjects;

public class ReceiptResponse
{
    public string TransactionHash { get; set; }
    public string BlockHash { get; set; }
    public string PostState { get; set; }
    public long? GasUsed { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string NewContractAddress { get; set; }
    public bool? Success { get; set; }
    public string ReturnValue { get; set; }
    public string Bloom { get; set; }
    public string Error { get; set; }
    public System.Collections.Generic.ICollection<LogResponse> Logs { get; set; }

}