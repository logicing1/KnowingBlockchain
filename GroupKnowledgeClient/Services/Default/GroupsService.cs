using Stratis.SmartContracts;

namespace GroupKnowledgeClient.Services.Default
{
    public class GroupsService : IGroupsService
    {
        public GroupsService()
        {
            var groupNames = new[]
            {
                "Cross-Enterprise Manufacturing",
                "WebAssembly Programing",
                "Quantum Field Theory",
                "Sumerian Antiquities",
                "Particle Swarm Optimization",
            };
            for (int i = 0; i < groupNames.Length; i++)
            {
                Groups.Add(MakeRandomAddress(i).ToString(), groupNames[i]);
            }
        }

        public event Action? GroupsChanged;

        public IDictionary<string, string> Groups { get; set; } = new Dictionary<string, string>();

        public string Connect(string addressHex)
        {
            var groupName = "General Aviation";
            Groups.Add(addressHex, groupName);
            NotifyChanged();
            return groupName;
        }

        public void Disconnect(string addressHex)
        {
            Groups.Remove(addressHex);
            NotifyChanged();
        }

        private void NotifyChanged() => GroupsChanged?.Invoke();

        private static Address MakeRandomAddress(int seed)
        {
            var random = new Random(seed);
            var pn1 = (uint)random.Next();
            var pn2 = (uint)random.Next();
            var pn3 = (uint)random.Next();
            var pn4 = (uint)random.Next();
            var pn5 = (uint)random.Next();
            return new Address(pn1, pn2, pn3, pn4, pn5);
        }

    }
}
