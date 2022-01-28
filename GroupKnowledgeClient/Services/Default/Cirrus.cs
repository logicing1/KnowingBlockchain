using System.Net.Http.Json;
using System.Text.Json;
using GroupKnowledgeClient.Model;
using Logicing.Knowing.StratisDataTransferObjects;

namespace GroupKnowledgeClient.Services.Default
{
    public class Cirrus : IBlockchain
    {
        private const string PROXY_ENDPOINT = "http://localhost:5203";

        private readonly HttpClient http;

        public Cirrus(string address)
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

            try
            {
                var request = new BuildCallContractTransactionRequest
                {
                    WalletName = agent.Wallet,
                    AccountName = agent.Account,
                    ContractAddress = Address,
                    MethodName = methodName,
                    Amount = "0",
                    FeeAmount = ".01",
                    Password = password,
                    GasPrice = agent.GasPrice,
                    GasLimit = agent.GasLimit,
                    Sender = agent.Address,
                    Parameters = parameters!
                };
                var response = await http.PostAsJsonAsync("http://localhost:5203/build-and-send-call", request);
                if (!response.IsSuccessStatusCode)
                    return string.Empty;
                var result = await response.Content.ReadFromJsonAsync<BuildCallContractTransactionResponse>() ?? new BuildCallContractTransactionResponse() { Success = false };
                return result.Success == true ? result.TransactionId : string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string?> GetTransactionResult(string txHash)
        {
            const string OPERATION = "receipt";

            try
            {
                var response = await http.GetAsync($"{OPERATION}?txHash={txHash}");
                if (!response.IsSuccessStatusCode)
                    return string.Empty;
                var result = await response.Content.ReadFromJsonAsync<ReceiptResponse>() ?? new ReceiptResponse() { Success = false };
                return result.Success == true ? result?.ReturnValue : string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<JsonElement?> MakeLocalCall(Agent agent, string methodName, ICollection<string>? parameters = null)
        {
            const string OPERATION = "local-call";

            try
            {
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}