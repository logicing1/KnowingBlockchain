using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State
{
    public interface IGroupState
    {
        IDictionary<string, Group> Connected { get; }

        Group? Selected { get; }

        event Func<Task>? Changed;

        Task Load();

        Task Select(string address);

        Question SelectedQuestion(string address);

        Task<bool> Connect(string groupAddress);

        Task Disconnect(Group group);
    }
}