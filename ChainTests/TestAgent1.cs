using GroupKnowledgeClient.Model;

namespace ChainTests
{
    public class TestAgent1 : Agent
    {
        public TestAgent1()
        {
            Wallet = "cirrusdev";
            Account = "account 0";
            Address = "PR26mueN928Ejujkrj9w3AMgcMfF8pvM8y";
        }

        public string Password { get; } = "password";
    }
}