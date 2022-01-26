using System.Data;
using System.Diagnostics;
using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Default;
using Stratis.SmartContracts;
using System.Net.Http.Json;


namespace GroupKnowledgeClient.Model
{
    public class Group
    {
        public static Group Empty { get; } = new(string.Empty, Agent.Empty);

        public static async Task<string> Establish(string name, ulong membershipFee)
        {
            throw new NotImplementedException();
        }

        private readonly CirrusApi contract;
        private readonly Agent agent;

        public Group(string contractAddress, Agent agent, string? name = default)
        {
            this.agent = agent;
            contract = new CirrusApi(contractAddress);
            Name = name ?? string.Empty;
        }

        public string Address => contract.Address;

        public string Name { get; private set; }

        public ulong MembershipFee { get; private set; } = 0;

        public ulong MemberBalance { get; private set; } = 0;

        public IList<Question> Questions { get; private set; } = new List<Question>();

        public async Task Load()
        {
            await LoadName();
            await LoadBalance();
            await LoadQuestions();
        }

        public async Task LoadName()
        {
            if (Name != string.Empty) return;
            var response = await contract.MakeLocalCall(agent, nameof(Name));
            if(!response.HasValue)
                return;
            Name = response.Value.GetString() ?? string.Empty;
        }

        public async Task LoadMembershipFee()
        {
            var response = await contract.MakeLocalCall(agent, nameof(MembershipFee));
            if(!response.HasValue)
                return;
            MembershipFee = response.Value.TryGetUInt64(out var fee) ? fee : 0;
                
        }

        public async Task LoadBalance()
        {
            var response = await contract.MakeLocalCall(agent, nameof(MemberBalance));
            if (!response.HasValue)
                return;
            MemberBalance = response.Value.TryGetUInt64(out var fee) ? fee : 0;
        }

        public async Task LoadQuestions()
        {
            var questions = new List<Question>();
            var response = await contract.MakeLocalCall(agent, nameof(Questions));
            if (!response.HasValue)
                return;
            var delimitedAddresses = response.Value.ToString();
            if (string.IsNullOrEmpty(delimitedAddresses)) 
                return;
            var questionAddresses = delimitedAddresses.Split(',');
            var index = 0;
            questions.AddRange(questionAddresses.Select(address => new Question(address, index++, agent)));
            Questions = questions;
        }

        public async Task<bool> Join(ulong fee, string password)
        {
            MemberBalance = fee;
            return true;
        }

        public async Task<bool> Withdraw(ulong tokens, string password)
        {
            MemberBalance = 0;
            return true;
        }

        public async Task<bool> Ask(string content, string password)
        {
            return true;
        }
    }
}
