using Newtonsoft.Json;

namespace Logicing.Knowing.StratisClient
{
    [JsonConverter(typeof(MoneyConverter))]
    public partial class Money { }
}