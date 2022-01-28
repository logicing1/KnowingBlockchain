using System;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Logicing.Knowing.UnitTests.Fakes;
using SimpleBase;
using Stratis.SmartContracts;
using Xunit;

namespace Logicing.Knowing.UnitTests.Contract
{
    public class GroupKnowledgeTests
    {
        private const string GROUP_NAME = "Test Group 01";

        private uint nextSender = 1;
        private ulong nextBlock = 1000;
        private ulong messageValue = 0;

        [Fact]
        public void CanEstablishWithName()
        {
            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            
            var group = new GroupKnowledge(state, GROUP_NAME);

            Assert.Equal(GROUP_NAME, group.Name());
        }

        [Fact]
        public void CanNotEstablishGroupWithInsufficientFunds()
        {
            messageValue = 999999;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);

            Assert.Throws<SmartContractAssertException>(() => new GroupKnowledge(state, GROUP_NAME));

        }

        [Fact]
        public void MembershipFeeEstablishedByCreator()
        {
            messageValue = 35000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);

            var group = new GroupKnowledge(state, GROUP_NAME);

            Assert.Equal(messageValue, group.MembershipFee());
        }

        [Fact]
        public void JoiningRequiresMembershipFee()
        {
            messageValue = 35000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var group = new GroupKnowledge(state, GROUP_NAME);
            messageValue = 1;

            Assert.Throws<SmartContractAssertException>(() => group.Join());
        }

        [Fact]
        public void JoiningIncreasesMembership()
        {
            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var group = new GroupKnowledge(state, GROUP_NAME);
            var membership = group.MembershipSize();
            nextSender++;

            group.Join();

            Assert.Equal(membership + 1, group.MembershipSize());
        }


        [Fact]
        public void WithdrawingAllTokensReducesMemberCount()
        {
            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var group = new GroupKnowledge(state, GROUP_NAME);
            var memberBalance = group.MemberBalance();
            var membership = group.MembershipSize();

            group.Withdraw(memberBalance);
            
            Assert.Equal(membership - 1, group.MembershipSize());

        }

        [Fact]
        public void AskAddsQuestion()
        {
            const string CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var group = new GroupKnowledge(state, GROUP_NAME);
            var startAsked = group.ListAsked().Length;

            group.Ask(CID);

            var asked = group.ListAsked();
            var endAsked = asked.Split(",").Length;
            Assert.Equal(startAsked + 1, endAsked);
        }


        private IMessage MakeMessage()
        {
            return new MessageFake(nextSender, messageValue);
        }

        private IBlock MakeBlock()
        {
            return new BlockFake(nextBlock++);
        }

        private string ToP2pkh(string addressHex)
        {
            const string VERSION = "37";

            var versionedAddress = Convert.FromHexString(VERSION + addressHex);
            var checkSum = SHA256.HashData(SHA256.HashData(versionedAddress)).Take(4);
            var p2pkh = versionedAddress.Concat(checkSum).ToArray();
            return Base58.Bitcoin.Encode(p2pkh);
        }

    }
}