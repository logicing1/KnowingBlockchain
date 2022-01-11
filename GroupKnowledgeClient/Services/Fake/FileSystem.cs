namespace GroupKnowledgeClient.Services.Fake
{
    public class FileSystem : Fake, IFileSystem
    {
        public FileSystem() : base(55)
        {
        }

        public string Retrieve(string cid)
        {
            throw new NotImplementedException();
        }

        public string Store(string content)
        {
            throw new NotImplementedException();
        }
    }
}
