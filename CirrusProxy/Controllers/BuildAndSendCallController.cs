using Microsoft.AspNetCore.Mvc;
using Logicing.Knowing.StratisDataTransferObjects;

namespace CirrusProxy.Controllers
{
    [ApiController]
    [Route($"/{OPERATION}")]
    public class BuildAndSendCallController : ControllerBase
    {
        private const string OPERATION = "build-and-send-call";

        private readonly ILogger<BuildCallContractTransactionRequest> logger;
        private readonly HttpClient http;

        public BuildAndSendCallController(ILogger<BuildCallContractTransactionRequest> logger, HttpClient http)
        {
            this.logger = logger;
            this.http = http;
        }

        [HttpPost]
        public async Task<BuildCallContractTransactionResponse> Post(BuildCallContractTransactionRequest request)
        {
            var response = await http.PostAsJsonAsync(OPERATION, request);
            var result = await response.Content.ReadFromJsonAsync<BuildCallContractTransactionResponse>();
            return result!;
        }
    }
}