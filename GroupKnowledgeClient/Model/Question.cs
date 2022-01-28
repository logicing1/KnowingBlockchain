using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Default;

namespace GroupKnowledgeClient.Model
{
    public class Question
    {
        public static Question Empty { get; } = new(string.Empty, -1, Agent.Empty);

        private const int TRANSACTION_DELAY = 5000;

        private readonly IFilestore fileSystem;
        private readonly IBlockchain blockchain;
        private readonly Agent agent;
        private string questionContent = string.Empty;

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

        public async Task LoadCid() 
        {
            if (Cid != string.Empty) return;
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
            var answersResponse = await blockchain.MakeLocalCall(agent, "ListAnswers");
            if(!answersResponse.HasValue) 
                return;
            var delimitedCid = answersResponse.Value.ToString();
            if (string.IsNullOrEmpty(delimitedCid)) 
                return;
            var answerCid = delimitedCid.Split(',');
            var index = 0;
            answers.AddRange(answerCid.Select(cid => new Answer(cid, index++)));
            Answers = answers;
        }

        public async Task LoadVote()
        {
            if(Answers.Count == 0) return;
            var voteResponse = await blockchain.MakeLocalCall(agent, "VoterVote");
            var delimitedRanks = voteResponse.ToString();
            if (string.IsNullOrEmpty(delimitedRanks)) return;
            var ranks = delimitedRanks.Split(",");
            for (var i = 0; i < Answers.Count; i++)
            {
                Answers[i].Rank = uint.Parse(ranks[i]);
            }
        }

        public async Task Load()
        {
            await LoadContent();
            await LoadAnswers();
        }

        public async Task<bool> Answer(string content, string password)
        {
            var cid = await fileSystem.Store(content);
            var transactionId = await blockchain.SendTransaction(agent, password, "ProposeAnswer", new string[] { $"4#{cid}" });
            await Task.Delay(TRANSACTION_DELAY);
            var address = await blockchain.GetTransactionResult(transactionId); //TODO: Answer based on transaction success, there is no return value
            return !string.IsNullOrWhiteSpace(address);
        }

        public void Rank(string password) 
        {
            throw new NotImplementedException();
        }


    }
}
