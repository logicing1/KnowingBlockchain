namespace Logicing.Knowing.StratisDataTransferObjects;

public class BuildCallContractTransactionRequest
{
    public string WalletName { get; set; }
    public string AccountName { get; set; }
    public ICollection<OutpointRequest> Outpoints { get; set; }
    public string ContractAddress { get; set; }
    public string MethodName { get; set; }
    public string Amount { get; set; }
    public string FeeAmount { get; set; }
    public string Password { get; set; }
    public long? GasPrice { get; set; }
    public long? GasLimit { get; set; }
    public string Sender { get; set; }
    public System.Collections.Generic.ICollection<string> Parameters { get; set; }
}