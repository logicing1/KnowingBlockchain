using System.Data;
using System.Diagnostics;
using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.SampleData;
using Stratis.SmartContracts;

namespace GroupKnowledgeClient.Model
{
    public class Group
    {
        public static Group Empty { get; } = new(string.Empty);

        public static async Task<string> Establish(string name, ulong membershipFee)
        {
            throw new NotImplementedException();
        }

        private readonly ContractProxy contract;
        private bool withdrawn = false;

        public Group(string contractAddress)
        {
            contract = new ContractProxy(contractAddress);
        }

        public string Address => contract.Address;

        public string Name { get; private set; } = string.Empty;

        public ulong MembershipFee { get; private set; } = 0;

        public ulong Balance { get; private set; }

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
            var response = await contract.MakeLocalCall(nameof(Name)) ?? string.Empty;
            Name = response.ToString() ?? string.Empty;
        }

        public async Task LoadMembershipFee()
        {
            var response = await contract.MakeLocalCall(nameof(MembershipFee)) ?? 0;
            MembershipFee = (ulong)response;
        }

        public async Task LoadBalance()
        {
            if (Balance > 0 || withdrawn) return;
            var response = await contract.MakeLocalCall(nameof(Balance)) ?? 0;
            Balance = (ulong)response;
        }

        public async Task LoadQuestions()
        {
            var questions = new List<Question>();
            var response = await contract.MakeLocalCall(nameof(Questions)) ?? string.Empty;
            var delimitedAddresses = response.ToString();
            if (string.IsNullOrEmpty(delimitedAddresses)) return;
            var questionAddresses = delimitedAddresses.Split(',');
            var index = 0;
            questions.AddRange(questionAddresses.Select(address => new Question(address, index++)));
            Questions = questions;
        }

        public async Task<bool> Join(ulong fee)
        {
            Balance = fee;
            return true;
        }

        public async Task<bool> Withdraw(ulong tokens)
        {
            Balance = 0;
            withdrawn = true;
            return true;
        }

        public async Task<bool> Ask(string content)
        {
            return true;
        }
    }
}
