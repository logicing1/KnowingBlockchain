using Newtonsoft.Json;

namespace Logicing.Deciding.StratisClient
{
    [JsonConverter(typeof(MoneyConverter))]
    public partial class Money { }
}