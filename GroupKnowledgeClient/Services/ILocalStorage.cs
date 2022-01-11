using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services
{
    public interface ILocalStorage
    {
        IList<Group> Groups { get; set; }
        string Wallet { get; set; }
    }
}