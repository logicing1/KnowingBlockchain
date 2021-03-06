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
        private const ulong ONE_COIN = 100000000;

        private uint nextSender = 1;
        private ulong nextBlock = 1000;
        private ulong messageValue = 0;

        [Fact]
        public void CanEstablishWithName()
        {
            messageValue = ONE_COIN;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            
            var group = new GroupKnowledge(state, GROUP_NAME);

            Assert.Equal(GROUP_NAME, group.Name());
        }

        [Fact]
        public void CanNotEstablishGroupWithInsufficientFunds()
        {
            messageValue = ONE_COIN - 1;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);

            Assert.Throws<SmartContractAssertException>(() => new GroupKnowledge(state, GROUP_NAME));

        }

        [Fact]
        public void MembershipFeeEstablishedByCreator()
        {
            messageValue = ONE_COIN * 2;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);

            var group = new GroupKnowledge(state, GROUP_NAME);

            Assert.Equal(messageValue, group.MembershipFee());
        }

        [Fact]
        public void JoiningRequiresMembershipFee()
        {
            messageValue = ONE_COIN;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var group = new GroupKnowledge(state, GROUP_NAME);
            messageValue = ONE_COIN - 1;

            Assert.Throws<SmartContractAssertException>(() => group.Join());
        }

        [Fact]
        public void JoiningIncreasesMembership()
        {
            messageValue = ONE_COIN;
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
            messageValue = ONE_COIN;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var group = new GroupKnowledge(state, GROUP_NAME);
            var memberBalance = group.MemberBalance();
            var membership = group.MembershipSize();

            group.Leave();
            
            Assert.Equal(membership - 1, group.MembershipSize());

        }

        [Fact]
        public void AskAddsQuestion()
        {
            const string CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";

            messageValue = ONE_COIN;
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


    }
}