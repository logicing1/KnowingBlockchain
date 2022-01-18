using GroupKnowledgeClient.Model;
using Logicing.Knowing.StratisClient;

namespace GroupKnowledgeClient.Services.Default
{
    public class ContractProxy : IContractProxy
    {
        protected const long DefaultGasPrice = 100;
        protected const long DefaultGasLimit = 100000;

        protected readonly ApiClient Api;
        protected readonly Agent Agent;

        public ContractProxy(string address)
        {
            Api = new ApiClient();
            Agent = new();
            Address = address;
        }

        public string Address { get; init; }

        public async Task<bool> VerifyWallet()
        {
            var request = new WalletLoadRequest
            {
                Name = Agent.Wallet,
                Password = Agent.Password,
            };
            try
            {
                await Api.ApiWalletLoadPostAsync(request);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<object> MakeLocalCall(string methodName, ICollection<string>? parameters = null)
        {
            var request = BuildLocalRequest(methodName, parameters);
            var response = await Api.ApiSmartContractsLocalCallPostAsync(request);
            return response.Return;
        }

        public async Task<Uint256> SendTransaction(string methodName, ICollection<string>? parameters = null)
        {
            var request = BuildTransactionRequest(methodName, parameters);
            var response = await Api.ApiSmartContractsBuildAndSendCallPostAsync(request);
            if (response?.Success == null || response.Success == false) throw new ApplicationException("Transaction Failed");
            return response.TransactionId;
        }

        public async Task<object> GetTransactionResult(Uint256 transaction)
        {
            var response = await Api.ApiSmartContractsReceiptGetAsync(transaction.Value);
            if (response?.Success == null || response.Success == false) throw new ApplicationException("Transaction Failed");
            return response.ReturnValue;
        }

        private LocalCallContractRequest BuildLocalRequest(string methodName, ICollection<string>? parameters = null)
        {
            var request = new LocalCallContractRequest
            {
                Sender = Agent.Address.ToString(),
                Amount = "0",
                GasPrice = DefaultGasPrice,
                GasLimit = DefaultGasLimit,
                ContractAddress = Address,
                MethodName = methodName
            };
            if (parameters != null) request.Parameters = parameters;
            return request;
        }

        private BuildCallContractTransactionRequest BuildTransactionRequest(string methodName, ICollection<string> parameters)
        {
            var request = new BuildCallContractTransactionRequest
            {
                Sender = Agent.Address,
                WalletName = Agent.Wallet,
                AccountName = Agent.Address,
                Password = Agent.Password,
                Amount = "0",
                FeeAmount = ".01",
                GasPrice = DefaultGasPrice,
                GasLimit = DefaultGasLimit,
                ContractAddress = Address,
                MethodName = methodName
            };
            if (parameters != null) request.Parameters = parameters;
            return request;
        }


    }
}