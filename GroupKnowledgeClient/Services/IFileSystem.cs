using GroupKnowledgeClient.Model;

namespace GroupKnowledgeClient.Services
{
    public interface IFileSystem
    {
        string Store(string content);
        string Retrieve(string cid);
    }
}
