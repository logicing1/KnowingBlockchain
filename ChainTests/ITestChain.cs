namespace ChainTests
{
    public interface ITestChain
    {
        long DefaultGasLimit { get; }
        long DefaultGasPrice { get; }
        string Location { get; }
        int TransactionTime { get; }
    }
}