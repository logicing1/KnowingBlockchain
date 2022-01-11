using Newtonsoft.Json;

namespace Logicing.Deciding.StratisClient
{
    [JsonConverter(typeof(Uint256Converter))]
    public partial class Uint256
    {
        public string Value { get; set; }
    }
}