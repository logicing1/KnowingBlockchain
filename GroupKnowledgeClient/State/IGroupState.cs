using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State
{
    public interface IGroupState
    {
        IDictionary<string, Group> Connected { get; }

        Group? Selected { get; set; }

        event Func<Task>? Changed;

        Question SelectQuestion(string address);

        Task<bool> Connect(string groupAddress);

        void Disconnect(Group group);
    }
}