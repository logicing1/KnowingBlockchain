using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Default;

namespace GroupKnowledgeClient.Model
{
    public class Question
    {
        public static Question Empty { get; } = new(string.Empty, -1, Agent.Empty);

        private readonly IFilestore fileSystem;
        private readonly IBlockchain blockchain;
        private readonly Agent agent;
        private string questionContent = string.Empty;
        private readonly TimeSpan transactionTime = TimeSpan.FromSeconds(15);

        public Question(string contractAddress, int index, Agent agent)
        {
            this.agent = agent;
            this.fileSystem = new InterPlanetaryFiles();
            blockchain = new Cirrus(contractAddress);
            Index = index;
        }

        public string Address => blockchain.Address;

        public int Index { get; init; }
         
        public string Cid { get; private set; } = string.Empty;

        public string Content { get; private set; } = string.Empty;

        public IList<Answer> Answers { get; private set; } = new List<Answer>();

        public Answer? BestAnswer { get; set; } = null;

        public async Task LoadCid() 
        {
            if (Cid != string.Empty) 
                return;
            var response = await blockchain.MakeLocalCall(agent, "Question");
            if(!response.HasValue)
                return;
            Cid = response.Value.ToString();
        }

        public async Task LoadContent()
        {
            if (Content != string.Empty) 
                return;
            if (Cid == string.Empty) 
                await LoadCid();
            Content = await fileSystem.Retrieve(Cid);
        }

        public async Task LoadAnswers()
        {
            var answers = new List<Answer>();
            var response = await blockchain.MakeLocalCall(agent, "ListAnswers");
            if(!response.HasValue) 
                return;
            var delimitedCid = response.Value.ToString();
            if (string.IsNullOrEmpty(delimitedCid)) 
                return;
            var answerCid = delimitedCid.Split(',');
            var index = 0;
            answers.AddRange(answerCid.Select(cid => new Answer(cid, index++)));
            Answers = answers;
            await LoadBest();
        }

        public async Task LoadBest()
        {
            var response = await blockchain.MakeLocalCall(agent, nameof(BestAnswer));
            if (!response.HasValue)
                return;
            var bestCid = response.Value.ToString();
            if(string.IsNullOrEmpty(bestCid))
                return;
            if (!Answers.Any())
                return;
            BestAnswer = Answers.FirstOrDefault(a => a.Cid == bestCid);
        }

        public async Task LoadVote()
        {
            if(Answers.Count == 0) 
                return;
            var voteResponse = await blockchain.MakeLocalCall(agent, "VoterVote");
            if (!voteResponse.HasValue)
                return;
            var delimitedRanks = voteResponse.Value.ToString();
            if (string.IsNullOrEmpty(delimitedRanks)) 
                return;
            var ranks = delimitedRanks.Split(",");
            for (var i = 0; i < ranks.Length; i++)
            {
                Answers[i].MemberRank = uint.Parse(ranks[i]);
            }
        }

        public async Task LoadScores()
        {
            if (Answers.Count == 0)
                return;
            var response = await blockchain.MakeLocalCall(agent, "AnswerScores");
            if (!response.HasValue)
                return;
            var delimitedScores = response.Value.ToString();
            if (string.IsNullOrEmpty(delimitedScores))
                return;
            var ranks = delimitedScores.Split(",");
            for (var i = 0; i < Answers.Count; i++)
            {
                Answers[i].GroupScore = int.Parse(ranks[i]);
            }
        }


        public async Task Load()
        {
            await LoadContent();
            await LoadAnswers();
            await LoadVote();
            await LoadScores();
        }

        public async Task<bool> Answer(string content, string password)
        {
            var cid = await fileSystem.Store(content);
            var transactionId = await blockchain.SendTransaction(agent, password, "ProposeAnswer", "0", new string[] { $"4#{cid}" });
            if (string.IsNullOrWhiteSpace(transactionId))
                return false;
            await Task.Delay(transactionTime);
            var (success, value) = await blockchain.GetTransactionResult(transactionId);
            return success;
        }

        public async Task<bool> Vote(string password)
        {
            var ranks = new uint[Answers.Count()];
            for (var i = 0; i < ranks.Length; i++)
            {
                ranks[i] = Answers[i].MemberRank;
            }
            var ballot = string.Join(',', ranks);
            var transactionId = await blockchain.SendTransaction(agent, password, "Vote", "0", new string[] { $"4#{ballot}" });
            await Task.Delay(transactionTime);
            var (success, value) = await blockchain.GetTransactionResult(transactionId);
            return success;
        }
    }
}
