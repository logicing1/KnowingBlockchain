using System;
using Stratis.SmartContracts;

[Deploy]
public class GroupKnowledge : SmartContract
{
    private const ulong MIN_MEMBERSHIP_FEE = 100000000; //Founder can establish higher
    private const int MIN_GROUP_NAME_LENGTH = 5;
    private const int MAX_GROUP_NAME_LENGTH = 50;
    private const int VOTING_REWARD_FACTOR = 100;  
    private const int ANSWER_REWARD_FACTOR = 5;
    private const ulong ASK_INTERVAL = 5;  //TODO: Change to longer interval once test/demo completed

    public GroupKnowledge(ISmartContractState contractState, string groupName) : base(contractState)
    {
        groupName = groupName?.Trim() ?? string.Empty;
        Assert(groupName.Length >= MIN_GROUP_NAME_LENGTH && groupName.Length <= MAX_GROUP_NAME_LENGTH, $"Group name must be between {MIN_GROUP_NAME_LENGTH} and {MAX_GROUP_NAME_LENGTH}.");
        GroupName = groupName;
        Assert(Message.Value >= MIN_MEMBERSHIP_FEE, $"The group's membership fee must be at least {MIN_MEMBERSHIP_FEE}.");
        GroupMembershipFee = Message.Value;
        SetMemberBalance(Message.Sender, Message.Value);
        TotalMembers = 1;
        TokenBalance = Message.Value;
        VoterReward = GroupMembershipFee / VOTING_REWARD_FACTOR;
        AnswererReward = VoterReward * ANSWER_REWARD_FACTOR;
        TotalAsked = 0;
        TotalUnanswered = 0;
        LastAskBlock = Block.Number > ASK_INTERVAL ? Block.Number - ASK_INTERVAL : 0;
    }

    private string GroupName
    {
        get => State.GetString(nameof(GroupName));
        set => State.SetString(nameof(GroupName), value);
    }

    private ulong GroupMembershipFee
    {
        get => State.GetUInt64(nameof(GroupMembershipFee));
        set => State.SetUInt64(nameof(GroupMembershipFee), value);
    }

    private ulong VoterReward
    {
        get => State.GetUInt64(nameof(VoterReward));
        set => State.SetUInt64(nameof(VoterReward), value);
    }

    private ulong AnswererReward
    {
        get => State.GetUInt64(nameof(AnswererReward));
        set => State.SetUInt64(nameof(AnswererReward), value);
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

    public ulong MemberBalance() => GetMemberBalance(Message.Sender);

    public bool IsMember(Address memberAddress) => GetMemberBalance(memberAddress) > 0;


    /// <summary>
    /// Message sender provides funds to become group member.
    /// </summary>
    public void Join()
    {
        Assert(Message.Value >= GroupMembershipFee, $"There is a minimum membership fee of {GroupMembershipFee} to join the group.");
        SetMemberBalance(Message.Sender, checked((TokenBalance * Message.Value)) / (Balance - Message.Value));
        TokenBalance += GetMemberBalance(Message.Sender);
        TotalMembers++;
    }


    /// <summary>
    /// Message sender withdraws funds equivalent to all their tokens and is no longer an active group member.
    /// </summary>
    public void Leave()
    {
        var result = Transfer(Message.Sender, checked((Balance * GetMemberBalance(Message.Sender))) / TokenBalance);
        Assert(result.Success, "Transfer failed.");
        SetMemberBalance(Message.Sender, 0);
        TotalMembers--;
    }


    /// <summary>
    /// Group member cashes out some of there tokens, but retains tokens equivalent to membership fee.
    /// </summary>
    /// <param name="tokens">Number of tokens to withdraw.</param>
    public void Withdraw(ulong tokens)
    {
        Assert(tokens <= GetMemberBalance(Message.Sender), "Insufficient token balance for requested withdrawal.");
        var minTokens = checked((TokenBalance * MembershipFee())) / Balance;
        Assert(GetMemberBalance(Message.Sender) - tokens >= minTokens, "Token balance can not go below current membership fee value");
        var result = Transfer(Message.Sender, checked((Balance * tokens)) / TokenBalance);
        Assert(result.Success, "Transfer failed.");
        SetMemberBalance(Message.Sender, GetMemberBalance(Message.Sender) - tokens);
        if (GetMemberBalance(Message.Sender) == 0) TotalMembers--;
    }


    /// <summary>
    /// Ask a question to the group. This will create a contract for that question will control selection of an answer.
    /// </summary>
    /// <param name="questionCid">
    ///     An Interplanetary File System (IPFS) Content Identifier (CID) that can be used to retrieve the content of the question.
    ///     The content identified by the CID is immutable, therefore once this question is added to the contract it can not be modified.
    /// </param>
    /// <returns>Hex value for the question contract that was created. This may be converted to P2PKH to call the question contract.</returns>
    public Address Ask(string questionCid)
    {
        Assert(Block.Number - LastAskBlock > ASK_INTERVAL, $"Only one question can be asked every {ASK_INTERVAL} blocks.");
        var result = Create<GroupKnowledgeQuestion>(0, new object[] { questionCid });
        Assert(result.Success, "Failed to create a contract for the question asked.");
        var questionContractAddress = result.NewContractAddress;
        SetQuestion(TotalAsked++, questionContractAddress);
        SetUnanswered(TotalUnanswered++, questionContractAddress);
        LastAskBlock = Block.Number;
        return questionContractAddress;
    }


    /// <summary>
    /// All the questions that have been asked of the group. 
    /// </summary>
    /// <returns>Question CIDs in a comma delimited string</returns>
    public string ListAsked()
    {
        var asked = new string[TotalAsked];
        for (var index = 0u; index < TotalAsked; index++)
        {
            asked[index] = GetAsked(index).ToString();
        }
        return string.Join(',', asked);
    }


    /// <summary>
    /// All the questions that have been asked of the group but have not had a final best answer selected. 
    /// </summary>
    /// <returns>Question CIDs in a comma delimited string</returns>
    public string ListUnanswered()
    {
        var unanswered = new string[TotalUnanswered];
        for (var index = 0u; index < TotalUnanswered; index++)
        {
            unanswered[index] = GetUnanswered(index).ToString();
        }
        return string.Join(',', unanswered);
    }


    /// <summary>
    /// Notification from a question contract that a member should get credit for voting.
    /// </summary>
    /// <param name="voter">Address of group member</param>
    public void Voted(Address voter)
    {
        var index = GetQuestionIndex(Message.Sender);
        Assert(GetUnanswered(index) == Message.Sender, "Not an unanswered question.");
        Assert(GetMemberBalance(voter) > 0, "Voter is not a current group member.");
        SetMemberBalance(voter, GetMemberBalance(voter) + VoterReward);
    }


    /// <summary>
    /// Notification from a question contract that the best answer for a question has been selected and who should get credit.
    /// </summary>
    /// <param name="answerer">Group member address</param>
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



/// <summary>
/// A smart contract used to determine the best answer to a group knowledge question.
/// </summary>
public class GroupKnowledgeQuestion : SmartContract
{
    private const string DEFAULT_ANSWER_CID = "bafyreiao4glgd4hejdc5suinjykavjmicaj5d5kckr7md6hetees2skksu"; //"Reject question as off topic, ambiguous, unanswerable, or otherwise invalid.";
    private const ulong FULL_PARTICIPATION_REQUIRED_PERIOD = 5;  //TODO: Reset to larger number after test and demo
    private const ulong PARTICIPATION_REQUIREMENT_REDUCTION_INTERVAL = 1; //TODO: Reset to larger number after test and demo
    private const uint MIN_PARTICIPATION_PERCENT = 25;
    private const uint MIN_REVOTE_PARTICIPATION_PERCENT = 75;

    public GroupKnowledgeQuestion(ISmartContractState contractState, string question) : base(contractState)
    {
        Group = Message.Sender;
        QuestionCid = question;
        ParticipationRequirement = MIN_PARTICIPATION_PERCENT;
        TotalAnswers = 0;
        SetAnswer(TotalAnswers++, DEFAULT_ANSWER_CID, Address, 0);
        BestAnswerCid = string.Empty;
        TotalVotes = 0;
        StartBlock = Block.Number;
    }

    private Address Group
    {
        get => State.GetAddress(nameof(Group));
        set => State.SetAddress(nameof(Group), value);
    }

    private string QuestionCid
    {
        get => State.GetString(nameof(QuestionCid));
        set => State.SetString(nameof(QuestionCid), value);
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

    private string BestAnswerCid
    {
        get => State.GetString(nameof(BestAnswerCid));
        set => State.SetString(nameof(BestAnswerCid), value);
    }


    /// <summary>
    /// The question this contract is about.
    /// </summary>
    /// <returns>A IPFS CID that can be used to get the content of the question.</returns>
    public string Question() => QuestionCid;


    /// <summary>
    /// Propose an answer to the contracted question.
    /// </summary>
    /// <param name="cid">
    ///     An Interplanetary File System (IPFS) Content Identifier (CID) that can be used to retrieve the content of the proposed answer.
    ///     The content identified by the CID is immutable, therefore once this answer is added to the contract it can not be modified.
    /// </param>
    public void ProposeAnswer(string cid)
    {
        var membershipCheckResult = Call(Group, 0, "IsMember", new object[] {Message.Sender});
        Assert(membershipCheckResult.Success && (bool)membershipCheckResult.ReturnValue == true, "Must be group member to propose an answer.");
        SetAnswer(TotalAnswers++, cid, Message.Sender, 0);
    }


    /// <summary>
    /// All the answers to the question that have been proposed. 
    /// </summary>
    /// <returns>Answer CIDs in a comma delimited string</returns>
    public string ListAnswers()
    {
        var asked = new string[TotalAnswers];
        for (var index = 0u; index < TotalAnswers; index++)
        {
            asked[index] = GetAnswer(index);
        }
        return string.Join(',', asked);
    }


    /// <summary>
    /// Submit a group member's ranking of currently proposed answers.
    /// </summary>
    /// <param name="ballot">
    ///     A list of unsigned integer ranks in order of the answer's sequential index value in a comma delimited string.
    ///     Ties are permitted, 0 indicates indifference. The number of answers ranked can be shorter than the current
    ///     number of answers in that it leaves off the most recently proposed answers. 
    /// </param>
    public void Vote(string ballot)
    {
        var vote = ReadBallot(ballot);
        var isRollback = TryRollback();
        SetVoterVote(Message.Sender, vote); 
        ApplyAnswerRankings(vote);
        if (!isRollback && BestAnswerCid.Length == 0) 
            RewardVote();
        TotalVotes++;
        if (ParticipationRequirementMet()) 
            FindBestAnswer();
    }


    /// <summary>
    /// The message sender's existing ranking of the proposed answers.
    /// </summary>
    /// <returns>A list of unsigned integer ranks in order of the answer's sequential index value in a comma delimited string.</returns>
    public string VoterVote()
    {
        var ranks = GetVoterVote(Message.Sender);
        var ballot = string.Join(',', ranks);
        return ballot;
    }

    /// <summary>
    /// The current scores for each answer based on voting up to this time.
    /// </summary>
    /// <returns>A list of signed integer ranks in order of the answer's sequential index value in a comma delimited string.  Lower is better, negative numbers best.</returns>
    public string AnswerScores()
    {
        var answers = new int[TotalAnswers];
        for (var index = 0u; index < TotalAnswers; index++)
        {
            answers[index] = GetAnswerScore(index);
        }
        return string.Join(',', answers);
    }


    /// <summary>
    /// The group's determination of the best answer to the question.
    /// </summary>
    /// <returns>An IPFS CID that can be used to get the content of the best answer.</returns>
    public string BestAnswer() => BestAnswerCid;


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
        if (vote.Length == 0) 
            return false;
        for (var voteIndex = 0; voteIndex < vote.Length; voteIndex++)
        {
            vote[voteIndex] = vote[voteIndex] * -1;
        }
        ApplyAnswerRankings(vote);
        return true;
    }

    private void ApplyAnswerRankings(int[] vote)
    {
        for (var answerIndex = 0u; answerIndex < vote.Length; answerIndex++)
        {
            var margins = 0;
            for (var voteIndex = 0u; voteIndex < vote.Length; voteIndex++)
            {
                if (voteIndex == answerIndex || vote[voteIndex] == 0) 
                    continue;
                margins += vote[answerIndex] - vote[voteIndex];
            }
            SetAnswerScore(answerIndex, margins);
        }
    }

    private bool ParticipationRequirementMet()
    {
        if (Block.Number - StartBlock < FULL_PARTICIPATION_REQUIRED_PERIOD) 
            return false;
        var result = Call(Group, 0, "MembershipSize");
        if (!result.Success) 
            return false;
        var membershipSize = (uint)result.ReturnValue;
        var requirement = 100 - (Block.Number - StartBlock) / PARTICIPATION_REQUIREMENT_REDUCTION_INTERVAL;
        if (requirement < ParticipationRequirement) 
            requirement = ParticipationRequirement;
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
                if (scores[j - 1] <= scores[j]) 
                    continue;
                var higherScore = scores[j - 1];
                var higherScoreAnswer = rankedAnswers[j - 1];
                scores[j - 1] = scores[j];
                rankedAnswers[j - 1] = rankedAnswers[j];
                scores[j] = higherScore;
                rankedAnswers[j] = higherScoreAnswer;
            }
        }
        if (scores.Length > 1 && scores[0] == scores[1]) 
            return; //Tie, no best answer yet
        if (!RewardAnswer(rankedAnswers[0])) 
            return; //Failed to reward as expected so don't resolve yet 
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
        if (BestAnswerCid.Length > 0) 
            return true; //Someone already rewarded
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

    private void SetAnswer(uint index, string cid, Address author, int score)
    {
        SetAnswerIndex(cid, index);
        SetAnswer(index, cid);
        SetAnswerAuthor(index, author);
        SetAnswerScore(index, score);
    }

    private uint GetAnswerIndex(string cid) => State.GetUInt32($"Answer:{cid}:Index");
    private void SetAnswerIndex(string cid, uint index) => State.SetUInt32($"Answer:{cid}:Index", index);

    private string GetAnswer(uint index) => State.GetString($"Answer:{index}");
    private void SetAnswer(uint index, string cid) => State.SetString($"Answer:{index}", cid);

    private Address GetAnswerAuthor(uint index) => State.GetAddress($"Answer:{index}:Author");
    private void SetAnswerAuthor(uint index, Address author) => State.SetAddress($"Answer:{index}:Author", author);

    private int GetAnswerScore(uint index) => State.GetInt32($"Answer:{index}:Score");
    private void SetAnswerScore(uint index, int score) => State.SetInt32($"Answer:{index}:Score", score);

    private Address GetVoter(uint index) => State.GetAddress($"Voter:{index}");
    private void SetVoter(uint index, Address voter) => State.SetAddress($"Voter:{index}", voter);

    private int[] GetVoterVote(Address member) => State.GetArray<int>($"Voter:{member}:Ballot");
    private void SetVoterVote(Address member, int[] ballot) => State.SetArray($"Voter:{member}:Ballot", ballot);

}
