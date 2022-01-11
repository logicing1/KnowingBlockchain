using Stratis.SmartContracts;

namespace GroupKnowledgeClient.Services.Fake
{
    public abstract class Fake
    {
        private readonly Random random;

        public Fake(int seed)
        {
            random = new Random(seed);
        }

        protected Address MakeRandomAddress()
        {
            var pn1 = (uint)random.Next();
            var pn2 = (uint)random.Next();
            var pn3 = (uint)random.Next();
            var pn4 = (uint)random.Next();
            var pn5 = (uint)random.Next();
            return new Address(pn1, pn2, pn3, pn4, pn5);
        }
        
        protected ulong MakeRandomBalance() => (ulong)random.Next();

        protected bool IsTrue() => random.Next() > int.MaxValue / 2;


    }
}
