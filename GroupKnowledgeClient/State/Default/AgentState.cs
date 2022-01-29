using Blazored.LocalStorage;
using GroupKnowledgeClient.Model;
using GroupKnowledgeClient.Shared;
using MudBlazor;

namespace GroupKnowledgeClient.State.Default
{
    public class AgentState : IAgentState
    {
        private readonly ILocalStorageService localStorage;

        public AgentState(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public event Func<Task>? Changed;

        public Agent ActiveAgent { get; private set; } = Agent.Empty;

        public bool TransactionInProgress { get; set; }

        public void StartTransaction()
        {
            TransactionInProgress = true;
            NotifyChanged();
        }

        public void StopTransaction()
        {
            TransactionInProgress = false;
            NotifyChanged();
        }

        public async Task Load()
        {
            var isStored = await localStorage.ContainKeyAsync(nameof(ActiveAgent));
            if (!isStored) 
                return;
            var agent = await localStorage.GetItemAsync<Agent>(nameof(ActiveAgent));
            ActiveAgent = agent;
            NotifyChanged();
        }

        public async Task Save(Agent agent)
        {
            await localStorage.SetItemAsync<Agent>(nameof(ActiveAgent), agent);
            await Load();
        }

        private void NotifyChanged() => Changed?.Invoke();

    }
}
