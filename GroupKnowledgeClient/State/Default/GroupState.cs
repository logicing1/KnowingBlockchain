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
            List = storage.Groups;
            Selected = default;
        }

        public event Action? Changed;

        public IList<Group> List { get; set; }

        public Group? Selected { get; set; }

        public Group? Connect(string contract)
        {
            var group = new Group(contract);
            NotifyChanged();
            return group;
        }

        public void Disconnect(Group group)
        {
            List.Remove(group);
            NotifyChanged();
        }

        private void NotifyChanged() => Changed?.Invoke();
    }
}
