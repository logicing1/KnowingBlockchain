using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services.Fake
{
    public class LocalStorage : Fake, ILocalStorage
    {
        public LocalStorage() : base(24)
        {
            Load();
        }

        public IList<Group> Groups { get; set; } = new List<Group>();

        public string Wallet { get; set; } = string.Empty;

        private void Load()
        {
            //var groupNames = new[]
            //{
            //    "Cross-Enterprise Manufacturing",
            //    "WebAssembly Programing",
            //    "Quantum Field Theory",
            //    "Sumerian Antiquities",
            //    "Particle Swarm Optimization",
            //};
        }


    }
}
