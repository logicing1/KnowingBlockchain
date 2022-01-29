using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State
{
    public interface IAgentState
    {
        event Func<Task>? Changed;

        Agent ActiveAgent { get; }

        bool TransactionInProgress { get; }

        void StartTransaction();

        void StopTransaction();

        Task Load();

        Task Save(Agent agent);
    }
}