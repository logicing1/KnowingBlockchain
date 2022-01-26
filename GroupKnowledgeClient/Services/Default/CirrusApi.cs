using System.Net;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using GroupKnowledgeClient.Model;
using Logicing.Knowing.StratisDataTransferObjects;
using Pinata.Client.Models;


namespace GroupKnowledgeClient.Services.Default
{
    public class CirrusApi : IBlockchain
    {
        private const string PROXY_ENDPOINT = "http://localhost:5203";

        private readonly HttpClient http;

        public CirrusApi(string address)
        {
            http = new HttpClient() { BaseAddress = new Uri(PROXY_ENDPOINT)};
            Address = address;
        }

        public string Address { get; init; }

        public async Task<string> CreateContract(Agent agent, string contractName, ICollection<string>? parameters = null)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SendTransaction(Agent agent, string password, string methodName, ICollection<string>? parameters = null)
        {
            const string OPERATION = "build-and-send-call";

            var request = new BuildCallContractTransactionRequest
            {
                WalletName = agent.Wallet,
                AccountName = agent.Account,
                ContractAddress = Address,
                MethodName = methodName,
                Amount = "0",
                Password = password,
                GasPrice = agent.GasPrice,
                GasLimit = agent.GasLimit,
                Sender = agent.Address
            };
            if (parameters != null) request.Parameters = parameters;
            var response = await http.PostAsJsonAsync(OPERATION, request);
            if (!response.IsSuccessStatusCode) return string.Empty;
            var result = await response.Content.ReadFromJsonAsync<BuildCallContractTransactionResponse>() ?? new BuildCallContractTransactionResponse() { Success = false };
            return result.Success == true ? result.TransactionId : string.Empty;
        }

        public async Task<object?> GetTransactionResult(string txHash)
        {
            const string OPERATION = "receipt";

            var response = await http.GetAsync($"{OPERATION}?txHash={txHash}");
            if(!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadFromJsonAsync<ReceiptResponse>() ?? new ReceiptResponse() { Success = false };
            if(!result.Success == true) return null;
            return result.ReturnValue;
        }

        public async Task<JsonElement?> MakeLocalCall(Agent agent, string methodName, ICollection<string>? parameters = null)
        {
            const string OPERATION = "local-call";

            var request = new LocalCallContractRequest
            {
                ContractAddress = Address,
                MethodName = methodName,
                Amount = "0",
                GasPrice = agent.GasPrice,
                GasLimit = agent.GasLimit,
                Sender = agent.Address,
            };
            var response = await http.PostAsJsonAsync(OPERATION, request);
            if (!response.IsSuccessStatusCode) 
                return null;
            var result = await response.Content.ReadFromJsonAsync<LocalExecutionResult>();
            return (JsonElement?)result?.Return;
        }
    }
}