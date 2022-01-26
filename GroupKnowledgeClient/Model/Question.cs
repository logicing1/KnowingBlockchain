using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Default;

namespace GroupKnowledgeClient.Model
{
    public class Question
    {
        public static Question Empty { get; } = new(string.Empty, -1, Agent.Empty);

        private readonly IFileSystem fileSystem;
        private readonly CirrusApi contract;
        private readonly Agent agent;
        private string questionContent = string.Empty;

        public Question(string contractAddress, int index, Agent agent)
        {
            this.agent = agent;
            this.fileSystem = new InterPlanetaryFiles();
            contract = new CirrusApi(contractAddress);
            Index = index;
        }

        public string Address => contract.Address;

        public int Index { get; init; }
         
        public string Cid { get; private set; } = string.Empty;

        public string Content { get; private set; } = string.Empty;

        public IList<Answer> Answers { get; private set; } = new List<Answer>();

        public async Task LoadCid() 
        {
            if (Cid != string.Empty) return;
            var response = await contract.MakeLocalCall(agent, nameof(Cid));
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
            var answersResponse = await contract.MakeLocalCall(agent, nameof(Answers));
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
            var voteResponse = await contract.MakeLocalCall(agent, "VoterVote");
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
            throw new NotImplementedException();
        }

        public void Rank(string password) 
        {
            throw new NotImplementedException();
        }


    }
}
