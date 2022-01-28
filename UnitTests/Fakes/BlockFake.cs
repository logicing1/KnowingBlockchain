using Stratis.SmartContracts;

namespace Logicing.Knowing.UnitTests.Fakes
{
    public class BlockFake : IBlock
    {
        public BlockFake(ulong number)
        {
            Coinbase = Address.Zero;
            Number = number;
        }

        public Address Coinbase { get; }

        public ulong Number { get; }
    }
}