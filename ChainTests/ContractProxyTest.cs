using System;
using System.Threading.Tasks;
using GroupKnowledgeClient.Services.Default;
using Stratis.SmartContracts;
using Xunit;

namespace ChainTests;

public class ContractProxyTest
{
    [Fact]
    public async Task LocalCallReturnsValue()
    {
        var contract = new TestContract01();
        var agent = new TestAgent1();
        var proxy = new CirrusApi(contract.Address);

        var response = await proxy.MakeLocalCall(agent, "Name");

        Assert.Equal(contract.Name, response.ToString());
    }

    [Fact]
    public async Task TransactionSucceeds()
    {
        var contract = new TestContract01();
        var agent = new TestAgent1();
        var proxy = new CirrusApi(contract.Address);

        var response = await proxy.SendTransaction(agent, agent.Password, "Name");

        //Assert.False(response.Empty);
    }

    [Fact]
    public async Task TransactionReceiptProvidesResult()
    {
        var chain = new TestChain1();
        var contract = new TestContract01();
        var agent = new TestAgent1();
        var proxy = new CirrusApi(contract.Address);

        var response = await proxy.SendTransaction(agent, agent.Password, "Name");
        await Task.Delay(chain.TransactionTime);
        var result = await proxy.GetTransactionResult(response);

        Assert.Equal(contract.Name, result.ToString());
    }



}