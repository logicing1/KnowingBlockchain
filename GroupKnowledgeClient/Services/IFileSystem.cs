using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services
{
    public interface IFileSystem
    {
        Task Store(string cid, string content);
        Task<string> Retrieve(string cid);
    }
}
