using System.Text.Json;
using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services
{
    public interface IBlockchain
    {
        string Address { get; init; }

        Task<string> CreateContract(Agent agent, string contractName, ICollection<string>? parameters = null);
        Task<string> SendTransaction(Agent agent, string password, string methodName, string amount, ICollection<string>? parameters = null);
        Task<(bool success, string? value)> GetTransactionResult(string transaction);
        Task<JsonElement?> MakeLocalCall(Agent agent, string methodName, ICollection<string>? parameters = null);
    }
}