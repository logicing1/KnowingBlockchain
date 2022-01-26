using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services
{
    public interface IFileSystem
    {
        Task<string> Store(string content);
        Task<string> Retrieve(string cid);
    }
}
