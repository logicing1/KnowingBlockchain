using Blazored.LocalStorage;
using GroupKnowledgeClient.Model;
using Stratis.SmartContracts;
using System.Linq;

namespace GroupKnowledgeClient.State.Default
{
    public class GroupState : IGroupState
    {
        private readonly ILocalStorageService localStorage;
        private readonly IAgentState agentState;

        public GroupState(ILocalStorageService localStorage, IAgentState agentState)
        {
            this.localStorage = localStorage;
            this.agentState = agentState;
        }

        public event Func<Task>? Changed;

        public IDictionary<string, Group> Connected { get; } = new Dictionary<string, Group>();

        public Group? Selected { get; private set; }

        public Question SelectedQuestion(string address) => Selected?.Questions.SingleOrDefault(q => q.Address == address) ?? Question.Empty;

        public async Task Update()
        {
            await Selected?.Load()!;
            NotifyChanged();
        }

        public async Task Select(string address)
        {
            if (!Connected.ContainsKey(address))
                return;
            Selected = Connected[address];
            await Selected.Load();
            NotifyChanged(); 
        }

        public async Task<bool> Connect(string groupAddress)
        {
            if (Connected.ContainsKey(groupAddress))
                return true;
            var group = new Group(groupAddress, agentState.ActiveAgent);
            await group.Load();
            if (group.Name == string.Empty)
                return false;
            Connected.Add(groupAddress, group);
            await Save();
            NotifyChanged();
            return true;
        }

        public async Task Disconnect(Group group)
        {
            Connected.Remove(group.Address);
            await Save();
            NotifyChanged();
        }

        public async Task Load()
        {
            Connected.Clear();
            var isStored = await localStorage.ContainKeyAsync(nameof(Connected));
            if (!isStored)
                return;
            var groups = await localStorage.GetItemAsync<GroupData[]>(nameof(Connected));
            if (!groups.Any())
                return;
            foreach (var group in groups)
            {
                Connected.Add(group.Address, new Group(group.Address, agentState.ActiveAgent, group.Name));
            }
            NotifyChanged();
        }

        public async Task Save()
        {
            await localStorage.RemoveItemAsync(nameof(Connected));
            var groups = Connected.Select(g => new GroupData { Address = g.Key, Name = g.Value.Name }).ToArray();
            await localStorage.SetItemAsync(nameof(Connected), groups);
        }

        private void NotifyChanged() => Changed?.Invoke();

        private struct GroupData
        {
            public string Address { get; init; }
            public string Name { get; init; }
        }
    }
}
