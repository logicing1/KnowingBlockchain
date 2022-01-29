using System.Data;
using System.Diagnostics;
using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Default;
using Stratis.SmartContracts;
using System.Net.Http.Json;
using System.Security.Cryptography;
using SimpleBase;


namespace GroupKnowledgeClient.Model
{
    public class Group
    {
        public static Group Empty { get; } = new(string.Empty, Agent.Empty);

        public static async Task<string> Establish(string name, ulong membershipFee)
        {
            throw new NotImplementedException();
        }

        private readonly IBlockchain blockchain;
        private readonly IFilestore fileSystem;
        private readonly Agent agent;
        private readonly TimeSpan transactionTime = TimeSpan.FromSeconds(15);

        public Group(string contractAddress, Agent agent, string? name = default)
        {
            this.agent = agent;
            blockchain = new Cirrus(contractAddress);
            fileSystem = new InterPlanetaryFiles();
            Name = name ?? string.Empty;
        }

        public string Address => blockchain.Address;

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
            var response = await blockchain.MakeLocalCall(agent, nameof(Name));
            if (!response.HasValue)
                return;
            Name = response.Value.GetString() ?? string.Empty;
        }

        public async Task LoadMembershipFee()
        {
            var response = await blockchain.MakeLocalCall(agent, nameof(MembershipFee));
            if (!response.HasValue)
                return;
            MembershipFee = response.Value.TryGetUInt64(out var fee) ? fee : 0;

        }

        public async Task LoadBalance()
        {
            var response = await blockchain.MakeLocalCall(agent, nameof(MemberBalance));
            if (!response.HasValue)
                return;
            MemberBalance = response.Value.TryGetUInt64(out var fee) ? fee : 0;
        }

        public async Task LoadQuestions()
        {
            var questions = new List<Question>();
            var response = await blockchain.MakeLocalCall(agent, "ListAsked");
            if (!response.HasValue)
                return;
            var delimitedAddresses = response.Value.ToString();
            if (string.IsNullOrEmpty(delimitedAddresses))
                return;
            var questionAddresses = delimitedAddresses.Split(',');
            var index = 0;
            questions.AddRange(questionAddresses.Select(address => new Question(ToP2Pkh(address), index++, agent)));
            Questions = questions;
        }

        public async Task<bool> Join(string amount, string password)
        {
            var transactionId = await blockchain.SendTransaction(agent, password, "Join", amount, new List<string>());
            if (string.IsNullOrEmpty(transactionId))
                return false;
            await Task.Delay(transactionTime);
            var (success, value) = await blockchain.GetTransactionResult(transactionId);
            return success;
        }

        public async Task<bool> Withdraw(ulong tokens, string password)
        {
            var transactionId = await blockchain.SendTransaction(agent, password, "Withdraw", "0", new string[] { $"7#{tokens.ToString()}" });
            if (string.IsNullOrEmpty(transactionId))
                return false;
            await Task.Delay(transactionTime);
            var (success, value) = await blockchain.GetTransactionResult(transactionId);
            return success;
        }

        public async Task<bool> Ask(string content, string password)
        {
            var cid = await fileSystem.Store(content);
            var transactionId = await blockchain.SendTransaction(agent, password, "Ask", "0", new string[] { $"4#{cid}" });
            if (string.IsNullOrEmpty(transactionId))
                return false;
            await Task.Delay(transactionTime);
            var (success, value) = await blockchain.GetTransactionResult(transactionId);
            if (success == false || value == string.Empty)
                return false;
            var p2Pkh = ToP2Pkh(value!);
            var question = new Question(p2Pkh, Questions.Count() + 1, agent);
            Questions.Add(question);
            return true;
        }

        private static string ToP2Pkh(string addressHex)
        {
            const string VERSION = "37";

            var versionedAddress = Convert.FromHexString(VERSION + addressHex);
            var checkSum = SHA256.HashData(SHA256.HashData(versionedAddress)).Take(4);
            var p2Pkh = versionedAddress.Concat(checkSum).ToArray();
            return Base58.Bitcoin.Encode(p2Pkh);
        }
    }
}
