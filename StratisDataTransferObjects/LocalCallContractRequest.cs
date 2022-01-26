namespace Logicing.Knowing.StratisDataTransferObjects;

public class LocalCallContractRequest
{
    public string ContractAddress { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string Amount { get; set; } = "0";
    public long? GasPrice { get; set; } = 0;
    public long? GasLimit { get; set; } = 0;
    public string Sender { get; set; } = string.Empty;
    public ICollection<string> Parameters { get; set; } = Array.Empty<string>();
}