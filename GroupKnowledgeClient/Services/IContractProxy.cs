using Logicing.Knowing.StratisClient;

namespace GroupKnowledgeClient.Services
{
    public interface IContractProxy
    {
        string Address { get; init; }

        Task<object> GetTransactionResult(Uint256 transaction);
        Task<object> MakeLocalCall(string methodName, ICollection<string>? parameters = null);
        Task<Uint256> SendTransaction(string methodName, ICollection<string>? parameters = null);
        Task<bool> VerifyWallet();
    }
}