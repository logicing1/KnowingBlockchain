using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.State.SampleData
{
    public class AgentState : IAgentState
    {
        public event Func<Task>? Changed;
        
        public Agent ActiveAgent { get; set; } = Agent.Empty;
        public async Task Load()
        {
            throw new NotImplementedException();
        }

        public async Task Save(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}
