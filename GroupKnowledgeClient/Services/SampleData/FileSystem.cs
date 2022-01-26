namespace GroupKnowledgeClient.Services.SampleData
{
    public class FileSystem : IFileSystem
    {
        private readonly IDictionary<string, string> files = new Dictionary<string, string>();

        public FileSystem()
        {
            Load(new SampleData());
        }

        public async Task<string> Store(string content)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Retrieve(string cid) => files.TryGetValue(cid, out var content) ? content : string.Empty;


        private void Load(SampleData sample)
        {
            foreach (var @group in sample.Groups.Values)
            {
                foreach (var question in @group.Questions)
                {
                    if(files.ContainsKey(question.CID)) continue;
                    files.Add(question.CID, question.Content);
                    foreach (var answer in question.Answers)
                    {
                        if(files.ContainsKey(answer.CID)) continue;
                        files.Add(answer.CID, answer.Content);
                    }
                }
            }
        }
    }
}
