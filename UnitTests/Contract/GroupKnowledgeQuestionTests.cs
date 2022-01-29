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
            var contract = new GroupKnowledgeQuestion(state, CID);

            var questionCid = contract.Question();

            Assert.Equal(CID, questionCid);

        }

        [Fact]
        public void CreatingQuestionAddsDefaultAnswer()
        {
            const string CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";
            const string DEFAULT_ANSWER = "bafyreiao4glgd4hejdc5suinjykavjmicaj5d5kckr7md6hetees2skksu";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var contract = new GroupKnowledgeQuestion(state, CID);

            var delimitedAnswers = contract.ListAnswers();

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
            var contract = new GroupKnowledgeQuestion(state, QUESTION_CID);

            contract.ProposeAnswer(ANSWER_CID);

            var delimitedAnswers = contract.ListAnswers();
            Assert.False(string.IsNullOrWhiteSpace(delimitedAnswers));
            var answers = delimitedAnswers.Split(",");
            Assert.Contains(ANSWER_CID, answers);
        }

        [Fact]
        public void VotingChangesMembersRankings()
        {
            const string QUESTION_CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";
            const string ANSWER1_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe111";
            const string ANSWER2_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe222";
            const string ANSWER3_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe333";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var contract = new GroupKnowledgeQuestion(state, QUESTION_CID);
            contract.ProposeAnswer(ANSWER1_CID);
            contract.ProposeAnswer(ANSWER2_CID);
            contract.ProposeAnswer(ANSWER3_CID);
            var ballot = "4,2,3,1";

            contract.Vote(ballot);

            var rankings = contract.VoterVote();
            Assert.Equal(ballot, rankings);
        }

        [Fact]
        public void VotingChangesAnswerScores()
        {
            const string QUESTION_CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";
            const string ANSWER1_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe111";
            const string ANSWER2_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe222";
            const string ANSWER3_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe333";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var contract = new GroupKnowledgeQuestion(state, QUESTION_CID);
            contract.ProposeAnswer(ANSWER1_CID);
            contract.ProposeAnswer(ANSWER2_CID);
            contract.ProposeAnswer(ANSWER3_CID);
            var ballot = "4,2,3,1";
            
            contract.Vote(ballot);

            var scores = contract.AnswerScores();
            Assert.True(scores[3] < scores[0]);
        }

        [Fact]
        public void CanVoteAfterAdditionalAnswersAdded()
        {
            const string QUESTION_CID = "bafyreig32ral25cg3sg5aypz5hjpe7vav5rj3mtzpw2zijcggamyhn3hnu";
            const string ANSWER1_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe111";
            const string ANSWER2_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe222";
            const string ANSWER3_CID = "bafyreigo6k32pasnq7ttd7236cpxn3xcbbbsbyj6zpbontrwv7lzsxe333";

            messageValue = 1000000;
            var state = new SmartContractStateFake(MakeMessage, MakeBlock);
            var contract = new GroupKnowledgeQuestion(state, QUESTION_CID);
            contract.ProposeAnswer(ANSWER1_CID);
            contract.ProposeAnswer(ANSWER2_CID);
            var ballot1 = "4,2,1";
            contract.Vote(ballot1);
            contract.ProposeAnswer(ANSWER3_CID);
            var ballot2 = "4, 2, 3, 1";
            
            contract.Vote(ballot2);

            var scores = contract.AnswerScores();
            Assert.True(scores[3] < scores[0]);
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