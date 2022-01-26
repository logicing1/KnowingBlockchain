namespace Logicing.Knowing.StratisDataTransferObjects;

public class TransferInfo
{
    public Uint160 From { get; set; }
    public Uint160 To { get; set; }
    public long? Value { get; set; }
}