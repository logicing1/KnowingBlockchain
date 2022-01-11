using System;
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
        private const int MIN_GROUPNAME_LENGTH = 5;
        private const int MAX_GROUPNAME_LENGTH = 50;
        private const int MIN_QUESTION_LENGTH = 100;
        private const int MAX_QUESTION_LENGTH = 5000;
        private const int ANSWER_REWARD_FACTOR = 4;

        public GroupKnowledge(ISmartContractState contractState, string groupName) : base(contractState)
        {
            groupName = groupName?.Trim() ?? string.Empty;
            Assert(groupName.Length >= MIN_GROUPNAME_LENGTH && groupName.Length <= MAX_GROUPNAME_LENGTH, $"Group name must be between {MIN_GROUPNAME_LENGTH} and {MAX_GROUPNAME_LENGTH}.");
            GroupName = groupName;
            Assert(Message.Value >= MIN_MEMBERSHIP_FEE, $"The group's membership fee must be at least {MIN_MEMBERSHIP_FEE}.");
            MembershipFee = Message.Value;
            SetMemberBalance(Message.Sender, Message.Value);
            TotalMembers++;
            TokenBalance = Message.Value;
            VoterReward = MembershipFee / MIN_MEMBERSHIP_FEE;
            AnswererReward = VoterReward * ANSWER_REWARD_FACTOR;
        }

        private string GroupName
        {
            get => State.GetString(nameof(GroupName));
            init => State.SetString(nameof(GroupName), value);
        }

        private ulong MembershipFee
        {
            get => State.GetUInt64(nameof(MembershipFee));
            init => State.SetUInt64(nameof(MembershipFee), value);
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

        public void Join()
        {
            Assert(IsMember(Message.Sender), "Already a member.");
            Assert(Message.Value >= MembershipFee, $"There is a minimum membership fee of {MembershipFee} to join the group.");
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

        public bool IsMember(Address memberAddress) => GetMemberBalance(memberAddress) > 0;

        public uint MembershipSize() => TotalMembers;

        public string Ask(string questionCid)
        {
            Assert(Block.Number - LastAskBlock > ASK_INTERVAL, $"Only one question can be asked every {ASK_INTERVAL} blocks.");
            var result = Create<GroupKnowledgeQuestion>(0, new object[] { Address, questionCid });
            Assert(result.Success, "Failed to create a contract for the question asked.");
            SetQuestion(++TotalAsked, questionCid, result.NewContractAddress);
            SetUnanswered(++TotalUnanswered, questionCid);
            return result.NewContractAddress.ToString();
        }

        public string ListAsked()
        {
            var asked = new string[TotalAsked];
            for (var index = 0u; index < TotalAsked; index++)
            {
                asked[index] = GetAsked(index);
            }
            return string.Join(',', asked);
        }

        public string ListUnanswered()
        {
            var unanswered = new string[TotalUnanswered];
            for (var index = 0u; index < TotalUnanswered; index++)
            {
                unanswered[index] = GetUnanswered(index);
            }
            return string.Join(',', unanswered);
        }

        public void Voted(string questionCid, Address voter)
        {
            Assert(Message.Sender == GetQuestionAddress(questionCid), "Only the question contract can report a vote.");
            Assert(GetMemberBalance(voter) > 0, "Voter is not a current group member.");
            SetMemberBalance(voter, GetMemberBalance(voter) + VoterReward);
        }

        public void Answered(Address answerer, string cid)
        {
            Assert(Message.Sender == GetQuestionAddress(cid), "Only the question contract can report an answer.");
            var index = GetQuestionIndex(Message.Sender);
            Assert(GetUnanswered(index) == cid, "Not an unanswered question.");
            ClearUnanswered(index);
            Assert(GetMemberBalance(answerer) > 0, "Answerer is not a current group member.");
            SetMemberBalance(answerer, GetMemberBalance(answerer) + AnswererReward);
        }

        private void SetQuestion(uint index, string cid, Address contractAddress)
        {
            SetAsked(index, cid);
            SetQuestionAddress(cid, contractAddress);
            SetQuestionIndex(Address, index);
        }

        private ulong GetMemberBalance(Address member) => State.GetUInt64($"Member:{member}:Balance");
        private void SetMemberBalance(Address member, ulong value) => State.SetUInt64($"Member:{member}:Balance", value);

        private Address GetQuestionAddress(string cid) => State.GetAddress($"Question:{cid}:Address");
        private void SetQuestionAddress(string cid, Address questionAddress) => State.SetAddress($"Question:{cid}:Address", Address);

        private uint GetQuestionIndex(Address questionAddress) => State.GetUInt32($"Question:{questionAddress}:Index");
        private void SetQuestionIndex(Address questionAddress, uint index) => State.SetUInt32($"Question:{questionAddress}:Index", index);

        private string GetAsked(uint index) => State.GetString($"Asked:{index}");
        private void SetAsked(uint index, string cid) => State.SetString($"Asked:{index}", cid);

        private string GetUnanswered(uint index) => State.GetString($"Unanswered:{index}:CID");
        private void SetUnanswered(uint index, string cid) => State.SetString($"Unanswered:{index}:CID", cid);
        private void ClearUnanswered(uint index) => State.Clear($"Unanswered:{index}:CID");
    }



    public class GroupKnowledgeQuestion : SmartContract
    {
        private const string DEFAULT_ANSWER = "Reject question as off topic, ambiguous, unanswerable, or otherwise invalid.";
        private const int MIN_ANSWER_LENGTH = 100;
        private const int MAX_ANSWER_LENGTH = 1000;
        private const ulong FULL_PARTICIPATION_REQUIRED_PERIOD = 5000;
        private const ulong PARTICIPATION_REQUIREMENT_REDUCTION_INTERVAL = 1000;
        private const uint MIN_PARTICIPATION_PERCENT = 25;
        private const uint MIN_REVOTE_PARTICIPATION_PERCENT = 75;

        public GroupKnowledgeQuestion(ISmartContractState contractState, byte[] question) : base(contractState)
        {
            Assert(question.Length == 32, "A valid hash of the question is required.");
            Group = Message.Sender;
            QuestionHash = question;
            ParticipationRequirement = MIN_PARTICIPATION_PERCENT;
            var defaultAnswerHash = Keccak256(Serializer.Serialize(DEFAULT_ANSWER));
            SetAnswer(++TotalAnswers, defaultAnswerHash, Address, int.MaxValue);
            StartBlock = Block.Number;
        }

        private Address Group
        {
            get => State.GetAddress(nameof(Group));
            init => State.SetAddress(nameof(Group), value);
        }

        private byte[] QuestionHash
        {
            get => State.GetBytes(nameof(QuestionHash));
            init => State.SetBytes(nameof(QuestionHash), value);
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

        private byte[] BestAnswerHash
        {
            get => State.GetBytes(nameof(BestAnswerHash));
            set => State.SetBytes(nameof(BestAnswerHash), value);
        }

        public string Question() => Serializer.ToString(QuestionHash);

        public string ProposeAnswer(string answer)
        {
            var membershipCheckResult = Call(Group, 0, "IsMember");
            Assert(membershipCheckResult.Success && (bool)membershipCheckResult.ReturnValue == true, "Must be group member to propose an answer.");
            answer = answer?.Trim() ?? string.Empty;
            Assert(answer.Length >= MIN_ANSWER_LENGTH && answer.Length <= MAX_ANSWER_LENGTH, $"Answer length must be between {MIN_ANSWER_LENGTH} and {MAX_ANSWER_LENGTH}.");
            var hash = Keccak256(Serializer.Serialize(answer));
            SetAnswer(++TotalAnswers, hash, Message.Sender, 0);
            return Serializer.ToString(hash);
        }

        public string ListAnswers()
        {
            var asked = new string[TotalAnswers];
            for (var index = 0u; index < TotalAnswers; index++)
            {
                asked[index] = Serializer.ToString(GetAnswerHash(index));
            }
            return string.Join(',', asked);
        }

        public void Vote(string ballot)
        {
            var vote = ReadBallot(ballot);
            var isRollback = TryRollback();
            ApplyAnswerRankings(vote);
            if (!isRollback && BestAnswerHash.Length == 0) RewardVote();
            TotalVotes++;
            if (ParticipationRequirementMet()) FindBestAnswer();
        }

        public string BestAnswer() => Serializer.ToString(BestAnswerHash);

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
            BestAnswerHash = GetAnswerHash(rankedAnswers[0]);
            ResetVoting();
        }

        private void RewardVote()
        {
            var result = Call(Group, 0, "Voted", new object[] { Message.Sender });
            Assert(result.Success, "Could not credit member for voting.");
        }

        private bool RewardAnswer(uint answerIndex)
        {
            if (BestAnswerHash.Length > 0) return true; //Someone already rewarded
            var bestAnswerAuthor = GetAnswerAuthor(answerIndex);
            var result = Call(Group, 0, "Answered", new object[] { QuestionHash, bestAnswerAuthor });
            return result.Success;
        }

        private void ResetVoting()
        {
            StartBlock = Block.Number;
            ParticipationRequirement = MIN_REVOTE_PARTICIPATION_PERCENT;
            TotalVotes = 0;
        }

        private void SetAnswer(uint index, byte[] hash, Address author, int score)
        {
            SetAnswerIndex(hash, index);
            SetAnswerHash(index, hash);
            SetAnswerAuthor(index, author);
            SetAnswerScore(index, score);
        }

        private uint GetAnswerIndex(byte[] hash) => State.GetUInt32($"Answer:{hash}:Index");
        private void SetAnswerIndex(byte[] hash, uint index) => State.SetUInt32($"Answer:{hash}:Index", index);

        private byte[] GetAnswerHash(uint index) => State.GetBytes($"Answer:{index}:Hash");
        private void SetAnswerHash(uint index, byte[] hash) => State.SetBytes($"Answer:{index}:Hash", hash);

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
