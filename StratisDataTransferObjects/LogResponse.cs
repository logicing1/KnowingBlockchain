namespace Logicing.Knowing.StratisDataTransferObjects;

public class LogResponse
{
    public string Address { get; set; }
    public ICollection<string> Topics { get; set; }
    public string Data { get; set; }

}