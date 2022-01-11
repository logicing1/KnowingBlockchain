using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State
{
    public interface IGroupState
    {
        IList<Group> List { get; set; }

        Group? Selected { get; set; }

        event Action? Changed;

        Group? Connect(string contract);

        void Disconnect(Group group);
    }
}