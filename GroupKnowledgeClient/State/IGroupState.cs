using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State
{
    public interface IGroupState
    {
        event Func<Task>? Changed;

        IDictionary<string, Group> Connected { get; }

        Group? Selected { get; }

        Task Load();

        Task Select(string address);

        Question SelectedQuestion(string address);

        Task<bool> Connect(string groupAddress);

        Task Disconnect(Group group);
    }
}