using System;
using System.Linq;
using System.Security.Cryptography;
using Logicing.Knowing.UnitTests.Fakes;
using SimpleBase;
using Stratis.SmartContracts;
using Xunit;

namespace Logicing.Knowing.UnitTests.Contract
{
    public class GroupKnowledgeQuestionTests
    {
        private const string GROUP_NAME = "Test Group 02";

        private uint nextSender = 1;
        private ulong nextBlock = 1000;
        private ulong messageValue = 0;

        [Fact]
        public void QuestionReturnsCid()
        {
            const string CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var questionContract = new GroupKnowledgeQuestion(state, CID);

            var questionCid = questionContract.Question();

            Assert.Equal(CID, questionCid);

        }

        [Fact]
        public void CreatingQuestionAddsDefaultAnswer()
        {
            const string CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";
            const string DEFAULT_ANSWER = "bafyreiao4glgd4hejdc5suinjykavjmicaj5d5kckr7md6hetees2skksu";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var questionContract = new GroupKnowledgeQuestion(state, CID);

            var delimitedAnswers = questionContract.ListAnswers();

            Assert.False(string.IsNullOrWhiteSpace(delimitedAnswers));
            var answers = delimitedAnswers.Split(",");
            Assert.Contains(DEFAULT_ANSWER, answers);
        }

        [Fact]
        public void ProposingAnswerAddsToList()
        {
            const string QUESTION_CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";
            const string ANSWER_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxefee";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var questionContract = new GroupKnowledgeQuestion(state, QUESTION_CID);

            questionContract.ProposeAnswer(ANSWER_CID);

            var delimitedAnswers = questionContract.ListAnswers();
            Assert.False(string.IsNullOrWhiteSpace(delimitedAnswers));
            var answers = delimitedAnswers.Split(",");
            Assert.Contains(ANSWER_CID, answers);

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