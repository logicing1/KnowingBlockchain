using System.Collections.Concurrent;
using Blazored.LocalStorage;
using GroupKnowledgeClient.Model;
using GroupKnowledgeClient.Services;

namespace GroupKnowledgeClient.State.SampleData
{
    public class GroupState : IGroupState
    {
        private readonly ILocalStorageService localStorage;

        public GroupState(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }
        
        public event Func<Task>? Changed;

        public IDictionary<string, Group> Connected { get; } = new ConcurrentDictionary<string, Group>();

        public Group? Selected { get; set; }

        public Question SelectQuestion(string address) => Selected?.Questions.SingleOrDefault(q => q.Address == address) ?? Question.Empty;

        public async Task<bool> Connect(string groupAddress)
        {
            var group = new Group(groupAddress, Agent.Empty);
            await group.LoadName();
            await group.LoadBalance();
            if (group.Name == string.Empty)
            {
                return false;
            }
            Connected.Add(groupAddress, group);
            NotifyChanged();
            return true;
        }

        public async Task Disconnect(Group @group)
        {
            Connected.Remove(@group.Address);
            NotifyChanged();
        }

        private void NotifyChanged() => Changed?.Invoke();

        public async Task Load()
        {
            throw new NotImplementedException();
        }

    }
}
