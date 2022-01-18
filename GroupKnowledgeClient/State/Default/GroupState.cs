using GroupKnowledgeClient.Model;
using GroupKnowledgeClient.Services;
using Stratis.SmartContracts;

namespace GroupKnowledgeClient.State.Default
{
    public class GroupState : IGroupState
    {
        private readonly ILocalStorage storage;

        public GroupState(ILocalStorage storage)
        {
            this.storage = storage;
        }

        public event Func<Task>? Changed;

        public Group? Selected { get; set; }

        public IDictionary<string, Group> Connected { get; } = new Dictionary<string, Group>();

        public Question SelectQuestion(string address) => Selected?.Questions.SingleOrDefault(q => q.Address == address) ?? Question.Empty;

        public async Task<bool> Connect(string groupAddress)
        {
            var group = new Group(groupAddress);
            await group.LoadName();
            await group.LoadBalance();
            if(group.Name == string.Empty) return false;
            Connected.Add(groupAddress, group);
            NotifyChanged();
            return true;
        }

        public void Disconnect(Group group)
        {
            Connected.Remove(group.Address);
            NotifyChanged();
        }

        private void NotifyChanged() => Changed?.Invoke();
    }
}
