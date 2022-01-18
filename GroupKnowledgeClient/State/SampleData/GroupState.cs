using System.Collections.Concurrent;
using GroupKnowledgeClient.Model;
using GroupKnowledgeClient.Services;

namespace GroupKnowledgeClient.State.SampleData
{
    public class GroupState : IGroupState
    {
        private readonly ILocalStorage localStorage;

        public GroupState(ILocalStorage localStorage)
        {
            this.localStorage = localStorage;
            foreach (var @group in localStorage.Groups)
            {
                Connected.Add(@group.Address, @group);
            }
        }
        
        public event Func<Task>? Changed;

        public IDictionary<string, Group> Connected { get; } = new ConcurrentDictionary<string, Group>();

        public Group? Selected { get; set; }

        public Question SelectQuestion(string address) => Selected?.Questions.SingleOrDefault(q => q.Address == address) ?? Question.Empty;

        public async Task<bool> Connect(string groupAddress)
        {
            var group = new Group(groupAddress);
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

        public void Disconnect(Group @group)
        {
            Connected.Remove(@group.Address);
            NotifyChanged();
        }

        private void NotifyChanged() => Changed?.Invoke();

    }
}
