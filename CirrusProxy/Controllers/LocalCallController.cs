using Microsoft.AspNetCore.Mvc;
using Logicing.Knowing.StratisDataTransferObjects;

namespace CirrusProxy.Controllers
{
    [ApiController]
    [Route($"/{OPERATION}")]
    public class LocalCallController : ControllerBase
    {
        private const string OPERATION = "local-call";

        private readonly ILogger<LocalCallController> logger;
        private readonly HttpClient http;

        public LocalCallController(ILogger<LocalCallController> logger, HttpClient http)
        {
            this.logger = logger;
            this.http = http;
        }

        [HttpPost]
        public async Task<LocalExecutionResult> Post(LocalCallContractRequest request)
        {
            var response = await http.PostAsJsonAsync(OPERATION, request);
            return await response.Content.ReadFromJsonAsync<LocalExecutionResult>() ?? new LocalExecutionResult();
        }
    }
}