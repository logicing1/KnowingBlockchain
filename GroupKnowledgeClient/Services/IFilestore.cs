using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services
{
    public interface IFilestore
    {
        Task<string> Store(string content);
        Task<string> Retrieve(string cid);
    }
}
