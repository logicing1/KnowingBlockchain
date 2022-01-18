namespace GroupKnowledgeClient.Services.SampleData
{
    public class FileSystem : IFileSystem
    {
        private readonly IDictionary<string, string> files = new Dictionary<string, string>();

        public FileSystem()
        {
            Load(new Sample());
        }

        public async Task<string> Retrieve(string cid) => files.TryGetValue(cid, out var content) ? content : string.Empty;

        public async Task Store(string cid, string content) => files.Add(cid, content);

        private void Load(Sample sample)
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
