using System.Collections.Generic;
using Stratis.SmartContracts;

namespace Logicing.Knowing.UnitTests.Fakes
{
    public class MessageFake : IMessage
    {
        public MessageFake(uint sender, ulong value = 0)
        {
            this.Sender = new Address(sender, sender, sender, sender, sender);
            this.Value = value;
        }

        public Address ContractAddress { get; }

        public Address Sender { get; init; }

        public ulong Value { get; set; }
    }
}