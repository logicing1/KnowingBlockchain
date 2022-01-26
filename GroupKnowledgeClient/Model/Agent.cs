using Google.Protobuf.WellKnownTypes;

namespace GroupKnowledgeClient.Model
{
    public class Agent
    {
        public static Agent Empty { get; } = new();

        public string Wallet { get; set; } = string.Empty;
        public string Account { get; set; } = "account 0";
        public string Address { get; set; } = string.Empty;
        public long GasPrice { get; set; } = 100;
        public long GasLimit { get; set; } = 200000;
    }
}
