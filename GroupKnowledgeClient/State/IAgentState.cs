using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State
{
    public interface IAgentState
    {
        event Func<Task>? Changed;

        Agent ActiveAgent { get; set; }

        Task Load();

        Task Save(Agent agent);
    }
}