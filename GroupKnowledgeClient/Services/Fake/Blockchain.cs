using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services.Fake
{
    public class Blockchain : Fake, IBlockchain
    {
        public Blockchain() : base(42) { }

    }
}
