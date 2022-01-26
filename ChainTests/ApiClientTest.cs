using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ChainTests
{
    public class ApiClientTest
    {
        [Fact]
        public async Task LocalCallReturnsValue()
        {
            var chain = new TestChain1();
            var contract = new TestContract01();
            var agent = new TestAgent1();
            var request = new LocalCallContractRequest
            {
                ContractAddress = contract.Address,
                MethodName = "Name",
                Amount = "0",
                GasPrice = chain.DefaultGasPrice,
                GasLimit = chain.DefaultGasLimit,
                Sender = agent.Address,
            };

            var api = new ApiClient(chain.Location, new HttpClient());
            var response = await api.ApiSmartContractsLocalCallPostAsync(request);

            Assert.Equal(contract.Name, response.Return.ToString());
        }

        [Fact]
        public async Task TransactionSucceeds()
        {
            var chain = new TestChain1();
            var contract = new TestContract01();
            var agent = new TestAgent1();
            var request = new BuildCallContractTransactionRequest
            {
                WalletName = agent.Wallet,
                AccountName = agent.Account,
                ContractAddress = contract.Address,
                MethodName = "Name",
                Amount = "0",
                FeeAmount = ".01",
                Password = agent.Password,
                GasPrice = chain.DefaultGasPrice,
                GasLimit = chain.DefaultGasLimit,
                Sender = agent.Address,
            };

            var api = new ApiClient(chain.Location, new HttpClient());
            var response = await api.ApiSmartContractsBuildAndSendCallPostAsync(request);

            Assert.True(response.Success);
        }

        [Fact]
        public async Task TransactionReceiptProvidesResult()
        {
            var chain = new TestChain1();
            var contract = new TestContract01();
            var agent = new TestAgent1();
            var request = new BuildCallContractTransactionRequest
            {
                WalletName = agent.Wallet,
                AccountName = agent.Account,
                ContractAddress = contract.Address,
                MethodName = "Name",
                Amount = "0",
                FeeAmount = ".01",
                Password = agent.Password,
                GasPrice = chain.DefaultGasPrice,
                GasLimit = chain.DefaultGasLimit,
                Sender = agent.Address,
            };
            var api = new ApiClient(chain.Location, new HttpClient());
            var response = await api.ApiSmartContractsBuildAndSendCallPostAsync(request);
            await Task.Delay(chain.TransactionTime);

            var receipt = await api.ApiSmartContractsReceiptGetAsync(response.TransactionId.Value);

            Assert.Equal(contract.Name, receipt.ReturnValue);

        }

    }
}
