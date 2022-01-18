using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Stratis.SmartContracts;
using Stratis.SmartContracts.Standards;

namespace GroupKnowledge.Contract
{
    [Deploy]
    public class GroupKnowledge : SmartContract
    {
        private const ulong MIN_MEMBERSHIP_FEE = 1000000;
        private const ulong ASK_INTERVAL = 100;
        private const int MIN_GROUP_NAME_LENGTH = 5;
        private const int MAX_GROUP_NAME_LENGTH = 50;
        private const int ANSWER_REWARD_FACTOR = 4;

        public GroupKnowledge(ISmartContractState contractState, string groupName) : base(contractState)
        {
            groupName = groupName?.Trim() ?? string.Empty;
            Assert(groupName.Length >= MIN_GROUP_NAME_LENGTH && groupName.Length <= MAX_GROUP_NAME_LENGTH, $"Group name must be between {MIN_GROUP_NAME_LENGTH} and {MAX_GROUP_NAME_LENGTH}.");
            GroupName = groupName;
            Assert(Message.Value >= MIN_MEMBERSHIP_FEE, $"The group's membership fee must be at least {MIN_MEMBERSHIP_FEE}.");
            GroupMembershipFee = Message.Value;
            SetMemberBalance(Message.Sender, Message.Value);
            TotalMembers++;
            TokenBalance = Message.Value;
            VoterReward = GroupMembershipFee / MIN_MEMBERSHIP_FEE;
            AnswererReward = VoterReward * ANSWER_REWARD_FACTOR;
        }

        private string GroupName
        {
            get => State.GetString(nameof(GroupName));
            init => State.SetString(nameof(GroupName), value);
        }

        private ulong GroupMembershipFee
        {
            get => State.GetUInt64(nameof(GroupMembershipFee));
            init => State.SetUInt64(nameof(GroupMembershipFee), value);
        }

        private ulong VoterReward
        {
            get => State.GetUInt64(nameof(VoterReward));
            init => State.SetUInt64(nameof(VoterReward), value);
        }

        private ulong AnswererReward
        {
            get => State.GetUInt64(nameof(AnswererReward));
            init => State.SetUInt64(nameof(AnswererReward), value);
        }

        private uint TotalMembers
        {
            get => State.GetUInt32(nameof(TotalMembers));
            set => State.SetUInt32(nameof(TotalMembers), value);
        }

        private ulong TokenBalance
        {
            get => State.GetUInt64(nameof(TokenBalance));
            set => State.SetUInt64(nameof(TokenBalance), value);
        }

        private uint TotalAsked
        {
            get => State.GetUInt32(nameof(TotalAsked));
            set => State.SetUInt32(nameof(TotalAsked), value);
        }

        private uint TotalUnanswered
        {
            get => State.GetUInt32(nameof(TotalUnanswered));
            set => State.SetUInt32(nameof(TotalUnanswered), value);
        }

        private ulong LastAskBlock
        {
            get => State.GetUInt64(nameof(LastAskBlock));
            set => State.SetUInt64(nameof(LastAskBlock), value);
        }

        public string Name() => GroupName;

        public ulong MembershipFee() => GroupMembershipFee;

        public uint MembershipSize() => TotalMembers;

        public bool IsMember(Address memberAddress) => GetMemberBalance(memberAddress) > 0;

        public void Join()
        {
            Assert(IsMember(Message.Sender), "Already a member.");
            Assert(Message.Value >= GroupMembershipFee, $"There is a minimum membership fee of {GroupMembershipFee} to join the group.");
            SetMemberBalance(Message.Sender, checked(TokenBalance * Message.Value) / Balance);
            TokenBalance += GetMemberBalance(Message.Sender);
            TotalMembers++;
        }

        public void Withdraw(ulong tokens)
        {
            Assert(tokens <= GetMemberBalance(Message.Sender), "Insufficient token balance for requested withdrawal.");
            var result = Transfer(Message.Sender, checked(Balance * GetMemberBalance(Message.Sender)) / TokenBalance);
            Assert(result.Success, "Transfer failed.");
            SetMemberBalance(Message.Sender, GetMemberBalance(Message.Sender) - tokens);
            if (GetMemberBalance(Message.Sender) == 0) TotalMembers--;
        }


        public void Ask(string questionCid)
        {
            Assert(Block.Number - LastAskBlock > ASK_INTERVAL, $"Only one question can be asked every {ASK_INTERVAL} blocks.");
            var result = Create<GroupKnowledgeQuestion>(0, new object[] { Address, questionCid });
            Assert(result.Success, "Failed to create a contract for the question asked.");
            SetQuestion(++TotalAsked, result.NewContractAddress);
            SetUnanswered(++TotalUnanswered, result.NewContractAddress);
        }

        public string ListAsked()
        {
            var asked = new string[TotalAsked];
            for (var index = 0u; index < TotalAsked; index++)
            {
                asked[index] = GetAsked(index).ToString();
            }
            return string.Join(',', asked);
        }

        public string ListUnanswered()
        {
            var unanswered = new string[TotalUnanswered];
            for (var index = 0u; index < TotalUnanswered; index++)
            {
                unanswered[index] = GetUnanswered(index).ToString();
            }
            return string.Join(',', unanswered);
        }

        public void Voted(Address voter)
        {
            var index = GetQuestionIndex(Message.Sender);
            Assert(GetUnanswered(index) == Message.Sender, "Not an unanswered question.");
            Assert(GetMemberBalance(voter) > 0, "Voter is not a current group member.");
            SetMemberBalance(voter, GetMemberBalance(voter) + VoterReward);
        }

        public void Answered(Address answerer)
        {
            var index = GetQuestionIndex(Message.Sender);
            Assert(GetUnanswered(index) == Message.Sender, "Not an unanswered question.");
            ClearUnanswered(index);
            Assert(GetMemberBalance(answerer) > 0, "Answerer is not a current group member.");
            SetMemberBalance(answerer, GetMemberBalance(answerer) + AnswererReward);
        }

        private void SetQuestion(uint index, Address questionAddress)
        {
            SetAsked(index, questionAddress);
            SetQuestionIndex(questionAddress, index);
        }

        private ulong GetMemberBalance(Address member) => State.GetUInt64($"Member:{member}:Balance");
        private void SetMemberBalance(Address member, ulong value) => State.SetUInt64($"Member:{member}:Balance", value);

        private uint GetQuestionIndex(Address questionAddress) => State.GetUInt32($"Question:{questionAddress}:Index");
        private void SetQuestionIndex(Address questionAddress, uint index) => State.SetUInt32($"Question:{questionAddress}:Index", index);

        private Address GetAsked(uint index) => State.GetAddress($"Asked:{index}");
        private void SetAsked(uint index, Address questionAddress) => State.SetAddress($"Asked:{index}", questionAddress);

        private Address GetUnanswered(uint index) => State.GetAddress($"Unanswered:{index}");
        private void SetUnanswered(uint index, Address questionAddress) => State.SetAddress($"Unanswered:{index}", questionAddress);
        private void ClearUnanswered(uint index) => State.Clear($"Unanswered:{index}");
    }



    public class GroupKnowledgeQuestion : SmartContract
    {
        private const string DEFAULT_ANSWER_CID_HEX = "0c312d1b5bb15fe25cda31f823a14f26aaa8981a524d92ba69069e684b85839e"; //"Reject question as off topic, ambiguous, unanswerable, or otherwise invalid.";
        private const ulong FULL_PARTICIPATION_REQUIRED_PERIOD = 5000;
        private const ulong PARTICIPATION_REQUIREMENT_REDUCTION_INTERVAL = 1000;
        private const uint MIN_PARTICIPATION_PERCENT = 25;
        private const uint MIN_REVOTE_PARTICIPATION_PERCENT = 75;

        public GroupKnowledgeQuestion(ISmartContractState contractState, byte[] question) : base(contractState)
        {
            Assert(question.Length == 32, "A valid hash of the question is required.");
            Group = Message.Sender;
            QuestionCid = question;
            ParticipationRequirement = MIN_PARTICIPATION_PERCENT;
            var defaultAnswerCid = HexStringToBytes(DEFAULT_ANSWER_CID_HEX);
            SetAnswer(++TotalAnswers, Serializer.Serialize(defaultAnswerCid), Address, int.MaxValue);
            StartBlock = Block.Number;
        }

        private Address Group
        {
            get => State.GetAddress(nameof(Group));
            init => State.SetAddress(nameof(Group), value);
        }

        private byte[] QuestionCid
        {
            get => State.GetBytes(nameof(QuestionCid));
            init => State.SetBytes(nameof(QuestionCid), value);
        }

        private ulong StartBlock
        {
            get => State.GetUInt64(nameof(StartBlock));
            set => State.SetUInt64(nameof(StartBlock), value);
        }

        private uint ParticipationRequirement
        {
            get => State.GetUInt32(nameof(ParticipationRequirement));
            set => State.SetUInt32(nameof(ParticipationRequirement), value);
        }

        private uint TotalAnswers
        {
            get => State.GetUInt32(nameof(TotalAnswers));
            set => State.SetUInt32(nameof(TotalAnswers), value);
        }

        private int TotalVotes
        {
            get => State.GetInt32(nameof(TotalVotes));
            set => State.SetInt32(nameof(TotalVotes), value);
        }

        private byte[] BestAnswerCid
        {
            get => State.GetBytes(nameof(BestAnswerCid));
            set => State.SetBytes(nameof(BestAnswerCid), value);
        }

        public byte[] CID() => QuestionCid;

        public void ProposeAnswer(byte[] cid)
        {
            var membershipCheckResult = Call(Group, 0, "IsMember");
            Assert(membershipCheckResult.Success && (bool)membershipCheckResult.ReturnValue == true, "Must be group member to propose an answer.");
            SetAnswer(++TotalAnswers, cid, Message.Sender, 0);
        }

        public string ListAnswers()
        {
            var asked = new string[TotalAnswers];
            for (var index = 0u; index < TotalAnswers; index++)
            {
                asked[index] = Serializer.ToString(GetAnswer(index));
            }
            return string.Join(',', asked);
        }

        public string VoterVote() => string.Join(',', GetVoterVote(Message.Sender));

        public void Vote(string ballot)
        {
            var vote = ReadBallot(ballot);
            var isRollback = TryRollback();
            ApplyAnswerRankings(vote);
            if (!isRollback && BestAnswerCid.Length == 0) RewardVote();
            TotalVotes++;
            if (ParticipationRequirementMet()) FindBestAnswer();
        }

        public string BestAnswer() => Serializer.ToString(BestAnswerCid);

        private int[] ReadBallot(string ballot)
        {
            var ranks = ballot.Split(',');
            Assert(ranks.Length <= TotalAnswers, "More ranks than there are answers to rank.");
            var vote = new int[TotalAnswers];
            for (var i = 0u; i < ranks.Length; i++)
            {
                Assert(uint.TryParse(ranks[i], out var rank));
                vote[i] = (int)rank;
            }
            return vote;
        }

        private bool TryRollback()
        {
            var vote = GetVoterVote(Message.Sender);
            if (vote.Length == 0) return false;
            for (var voteIndex = 0; voteIndex < vote.Length; voteIndex++)
            {
                vote[voteIndex] = vote[voteIndex] * -1;
            }
            ApplyAnswerRankings(vote);
            return true;
        }

        private void ApplyAnswerRankings(int[] vote)
        {
            for (var answerIndex = 0u; answerIndex < TotalAnswers; answerIndex++)
            {
                var margins = 0;
                for (var voteIndex = 0u; voteIndex < vote.Length; voteIndex++)
                {
                    if (voteIndex == answerIndex || vote[voteIndex] == 0) continue;
                    margins += vote[answerIndex] - vote[voteIndex];
                }
                SetAnswerScore(answerIndex, margins);
            }
        }

        private bool ParticipationRequirementMet()
        {
            if (Block.Number - StartBlock < FULL_PARTICIPATION_REQUIRED_PERIOD) return false;
            var result = Call(Group, 0, "MembershipSize");
            if (!result.Success) return false;
            var membershipSize = (uint)result.ReturnValue;
            var requirement = 100 - (Block.Number - StartBlock) / PARTICIPATION_REQUIREMENT_REDUCTION_INTERVAL;
            if (requirement < ParticipationRequirement) requirement = ParticipationRequirement;
            return (ulong)TotalVotes * 100 >= membershipSize * requirement;
        }

        private void FindBestAnswer()
        {
            var rankedAnswers = new uint[TotalAnswers];
            var scores = new int[TotalAnswers];
            for (var index = 0u; index < TotalAnswers; index++)
            {
                rankedAnswers[index] = index;
                scores[index] = GetAnswerScore(index);
            }
            for (var i = 0u; i < TotalAnswers - 1; i++)
            {
                for (var j = i + 1; j > 0; j--)
                {
                    if (scores[j - 1] <= scores[j]) continue;
                    var higherScore = scores[j - 1];
                    var higherScoreAnswer = rankedAnswers[j - 1];
                    scores[j - 1] = scores[j];
                    rankedAnswers[j - 1] = rankedAnswers[j];
                    scores[j] = higherScore;
                    rankedAnswers[j] = higherScoreAnswer;
                }
            }
            if (scores.Length > 1 && scores[0] == scores[1]) return; //Tie, no best answer yet
            if (!RewardAnswer(rankedAnswers[0])) return; //Failed to reward as expected so don't resolve yet 
            BestAnswerCid = GetAnswer(rankedAnswers[0]);
            ResetVoting();
        }

        private void RewardVote()
        {
            var result = Call(Group, 0, "Voted", new object[] { Message.Sender });
            Assert(result.Success, "Could not credit member for voting.");
        }

        private bool RewardAnswer(uint answerIndex)
        {
            if (BestAnswerCid.Length > 0) return true; //Someone already rewarded
            var bestAnswerAuthor = GetAnswerAuthor(answerIndex);
            var result = Call(Group, 0, "Answered", new object[] { bestAnswerAuthor });
            return result.Success;
        }

        private void ResetVoting()
        {
            StartBlock = Block.Number;
            ParticipationRequirement = MIN_REVOTE_PARTICIPATION_PERCENT;
            TotalVotes = 0;
        }

        private void SetAnswer(uint index, byte[] cid, Address author, int score)
        {
            SetAnswerIndex(cid, index);
            SetAnswer(index, cid);
            SetAnswerAuthor(index, author);
            SetAnswerScore(index, score);
        }

        private static byte[] HexStringToBytes(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2)
            {
                var hexChars = hex.Substring(i, 2);
                bytes[i / 2] = byte.Parse(hexChars, System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }


        private uint GetAnswerIndex(byte[] cid) => State.GetUInt32($"Answer:{cid}:Index");
        private void SetAnswerIndex(byte[] cid, uint index) => State.SetUInt32($"Answer:{cid}:Index", index);

        private byte[] GetAnswer(uint index) => State.GetBytes($"Answer:{index}");
        private void SetAnswer(uint index, byte[] cid) => State.SetBytes($"Answer:{index}", cid);

        private Address GetAnswerAuthor(uint index) => State.GetAddress($"Answer:{index}:Author");
        private void SetAnswerAuthor(uint index, Address author) => State.SetAddress($"Answer:{index}:Author", author);

        private int GetAnswerScore(uint index) => State.GetInt32($"Answer:{index}:Score");
        private void SetAnswerScore(uint index, int score) => State.SetInt32($"Answer:{index}:Score", score);

        private Address GetVoter(uint index) => State.GetAddress($"Voter:{index}");
        private void SetVoter(uint index, Address voter) => State.SetAddress($"Voter:{index}", voter);

        private int[] GetVoterVote(Address member) => State.GetArray<int>($"Voter:{member}:Ballot");
        private void SetVoterVote(Address member, int[] ballot) => State.SetArray($"Voter:{member}:Ballot", ballot);

    }
}
