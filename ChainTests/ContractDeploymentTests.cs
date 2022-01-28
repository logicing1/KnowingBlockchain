using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GroupKnowledgeClient.Model;
using ICSharpCode.Decompiler.IL;
using Logicing.Knowing.StratisDataTransferObjects;
using Xunit;
using Stratis.SmartContracts.CLR.Compilation;
using Stratis.SmartContracts.Core;
using Xunit.Abstractions;

namespace ChainTests;

public class ContractDeploymentTests
{
    //Set .cs file's Copy to Output Directory property to Copy always
    private const string CONTRACT_CODE_FILE = "GroupKnowledge.cs";
    private const string API_ENDPOINT = @"http://localhost:38223/api/SmartContracts/";


    private readonly ITestOutputHelper testOutputHelper;
    private readonly Agent agent;
    private readonly ITestChain chain;
    private readonly string password;
    private readonly string outputLocation;

    public ContractDeploymentTests(ITestOutputHelper testOutputHelper)
    {
        this.testOutputHelper = testOutputHelper;
        this.agent = new TestAgent1();
        this.chain = new TestChain1();
        this.password = "password";
        outputLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    }

    [Fact]
    public void ContractCompiles()
    {
        var compilationResult = ContractCompiler.CompileFile(CONTRACT_CODE_FILE);
        Assert.True(compilationResult.Success);
        OutputByteCode(compilationResult);
    }

    [Fact]
    public void ContractValidates()
    {
        var compilationResult = ContractCompiler.CompileFile(CONTRACT_CODE_FILE);
        var module = ContractDecompiler.GetModuleDefinition(compilationResult.Compilation).Value;
        var validator = new Stratis.SmartContracts.CLR.Validation.SmartContractValidator();
        var results = validator.Validate(module.ModuleDefinition);
        foreach (var error in results.Errors)
        {
            testOutputHelper.WriteLine(error.Message);
        }
        Assert.Empty(results.Errors);
    }

    [Fact]
    public async Task ContractDeploys()
    {
        const string OPERATION = "build-and-send-create";
        const string GROUP_NAME = "Question Contract 02";

        var compilationResult = ContractCompiler.CompileFile(CONTRACT_CODE_FILE);
        var byteCode = compilationResult.Compilation.ToHexString();
        var request = new BuildCreateContractTransactionRequest
        {
            WalletName = agent.Wallet,
            AccountName = agent.Account,
            Amount = ".01",
            FeeAmount = ".001",
            Password = password,
            ContractCode = byteCode,
            GasPrice = agent.GasPrice,
            GasLimit = 250000,
            Sender = agent.Address,
            Parameters = new string[] { $"4#{GROUP_NAME}" }
        };
        var http = new HttpClient() { BaseAddress = new Uri(API_ENDPOINT) };
        var response = await http.PostAsJsonAsync(OPERATION, request);
        Assert.True(response.IsSuccessStatusCode);
    }

    private void OutputByteCode(ContractCompilationResult compilationResult)
    {
        var byteCode = compilationResult.Compilation.ToHexString();
        var outputFile = Path.Combine(outputLocation, $"{CONTRACT_CODE_FILE}_ByteCode_{DateTime.Now.Ticks}.txt");
        File.WriteAllText(outputFile, byteCode);
    }


}