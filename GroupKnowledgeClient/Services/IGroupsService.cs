
namespace GroupKnowledgeClient.Services
{
    public interface IGroupsService
    {
        IDictionary<string, string> Groups { get; set; }

        event Action? GroupsChanged;

        string Connect(string addressHex);
        void Disconnect(string addressHex);
    }
}