using Stratis.SmartContracts;

namespace Logicing.Knowing.UnitTests.Fakes
{
    public class TransferResultFake : ITransferResult
    {
        public object ReturnValue { get; }
        public bool Success { get; } = true;
    }
}