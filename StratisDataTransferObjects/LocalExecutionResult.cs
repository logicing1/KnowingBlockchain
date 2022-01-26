namespace Logicing.Knowing.StratisDataTransferObjects;

public class LocalExecutionResult
{
    public ICollection<TransferInfo> InternalTransfers { get; set; }
    public Gas GasConsumed { get; set; }
    public bool? Revert { get; set; }
    public ContractErrorMessage ErrorMessage { get; set; }
    public object Return { get; set; }
    public ICollection<Log> Logs { get; set; }
}