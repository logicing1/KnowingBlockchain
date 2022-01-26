using Flurl.Http;
using Logicing.Knowing.StratisClient;
using System.Linq;
using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services.SampleData
{
    public class ContractProxy : IContractProxy
    {
        private readonly SampleData sample = new();

        public ContractProxy(string address)
        {
            Address = address;
        }

        public string Address { get; init; }

        public async Task<object> MakeLocalCall(string methodName, ICollection<string>? parameters = null) => MakeLocalCall(Agent.Empty, methodName, parameters);

        public async Task<object> MakeLocalCall(Agent agent, string methodName, ICollection<string>? parameters = null)
        {
            return methodName switch
            {
                "Address" => Address,
                "Name" => sample.Groups[Address].Name,
                "MembershipFee" => 10000ul,
                "Balance" => sample.Groups[Address].Balance,
                "Questions" => string.Join(',', sample.Groups[Address].Questions.Select(q => q.Address)),
                "Cid" => QuestionCid(),
                "Answers" => AnswerCiDs(),
                "VoterVote" => VoterVote(),
                _ => string.Empty
            };
        }

        public async Task<Uint256> SendTransaction(Agent agent, string password, string methodName, ICollection<string>? parameters = null)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetTransactionResult(Uint256 transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> VerifyWallet(Agent agent)
        {
            throw new NotImplementedException();
        }

        private string QuestionCid ()
        {
            foreach (var group in sample.Groups.Values)
            {
                foreach (var question in group.Questions)
                {
                    if (question.Address == Address)
                    {
                        return question.CID;
                    }
                }
            }
            return string.Empty;
        }

        private string AnswerCiDs()
        {
            foreach (var group in sample.Groups.Values)
            {
                foreach (var question in group.Questions)
                {
                    if (question.Address == Address) return string.Join(',', question.Answers.Select(a => a.CID));
                }
            }
            return string.Empty;
        }

        private string VoterVote()
        {
            foreach (var group in sample.Groups.Values)
            {
                foreach (var question in group.Questions)
                {
                    if (question.Address == Address) return string.Join(',', question.Answers.Select(a => a.VoterRank));
                }
            }
            return string.Empty;
        }









    }
}
