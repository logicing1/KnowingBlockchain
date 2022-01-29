using NSubstitute.Routing.Handlers;
using Stratis.SmartContracts;

namespace Logicing.Knowing.UnitTests.Fakes
{
    public class TransferResultFake : ITransferResult
    {
        public TransferResultFake(object returnValue)
        {
            ReturnValue = returnValue;
        }
        public object ReturnValue { get; }
        public bool Success { get; } = true;
    }
}