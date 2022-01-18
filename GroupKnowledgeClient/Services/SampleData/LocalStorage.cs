using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services.SampleData
{
    public class LocalStorage : ILocalStorage
    {
        private const int TOTAL_CONNECTED = 3;

        private readonly Sample sample = new();

        public LocalStorage()
        {
            var connectedCount = 0;
            foreach (var group in sample.Groups.Values)
            {
                Groups.Add(new Group(group.Address));
                connectedCount++;
                if(connectedCount >= TOTAL_CONNECTED) break;
            }
        }

        public IList<Group> Groups { get; set; } = new List<Group>();

        public string Wallet { get; set; } = string.Empty;



    }
}
