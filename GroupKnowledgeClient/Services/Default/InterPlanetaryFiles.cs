using System.Net.Http.Json;


namespace GroupKnowledgeClient.Services.Default
{
    public class InterPlanetaryFiles : IFilestore
    {
        private const string IPFS_ENDPOINT = "http://localhost:5001/api/v0/";

        private HttpClient http;

        public InterPlanetaryFiles()
        {
            http = new HttpClient() { BaseAddress = new Uri(IPFS_ENDPOINT) };
        }

        public async Task<string> Retrieve(string cid)
        {
            const string OPERATION = "dag/get";

            var response = await http.PostAsync($"{OPERATION}?arg={cid}", null);
            var content = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : string.Empty;
            return content;
        }

        public async Task<string> Store(string text)
        {
            const string OPERATION = "dag/put?pin=true";

            var content = new MultipartFormDataContent { JsonContent.Create(text) };
            var response = await http.PostAsync(OPERATION, content);
            var result = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : string.Empty;
            var cid = !string.IsNullOrEmpty(result) ? result.Split(":").Last()[1..^4] : string.Empty;
            return cid;
        }
    }
}
