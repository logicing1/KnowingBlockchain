namespace Logicing.Knowing.StratisDataTransferObjects;

public class BuildCreateContractTransactionRequest
{
    [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
    public string WalletName { get; set; }
    public string AccountName { get; set; }
    public System.Collections.Generic.ICollection<OutpointRequest> Outpoints { get; set; }
    public string Amount { get; set; }
    public string FeeAmount { get; set; }
    public string Password { get; set; }
    public string ContractCode { get; set; }
    public long? GasPrice { get; set; }
    public long? GasLimit { get; set; }
    public string Sender { get; set; }
    public System.Collections.Generic.ICollection<string> Parameters { get; set; }
}