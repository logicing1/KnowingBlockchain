namespace Logicing.Knowing.StratisDataTransferObjects;

public class Log
{
    public Uint160 Address { get; set; }
    public ICollection<byte[]> Topics { get; set; }
    public byte[] Data { get; set; }
}