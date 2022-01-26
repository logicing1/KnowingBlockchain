using Microsoft.AspNetCore.Mvc;
using Logicing.Knowing.StratisDataTransferObjects;

namespace CirrusProxy.Controllers
{
    [ApiController]
    [Route($"/{OPERATION}")]
    public class ReceiptController : ControllerBase
    {
        private const string OPERATION = "receipt";

        private readonly ILogger<LocalCallController> logger;
        private readonly HttpClient http;

        public ReceiptController(ILogger<LocalCallController> logger, HttpClient http)
        {
            this.logger = logger;
            this.http = http;
        }

        [HttpGet]
        public async Task<ReceiptResponse> Post(string txHash)
        {
            var response = await http.GetAsync($"{OPERATION}?txHash={txHash}");
            return await response.Content.ReadFromJsonAsync<ReceiptResponse>() ?? new ReceiptResponse();
        }
    }
}