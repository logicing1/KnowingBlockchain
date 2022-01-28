using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State
{
    public interface IAgentState
    {
        event Func<Task>? Changed;

        Agent ActiveAgent { get; }

        Task Load();

        Task Save(Agent agent);
    }
}