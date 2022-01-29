 using GroupKnowledgeClient.Services.Default;

namespace GroupKnowledgeClient.Model
{
    public class Answer
    {
        public static Answer Empty { get; } = new(string.Empty, -1);

        private readonly InterPlanetaryFiles fileSystem;

        public Answer(string cid, int index)
        {
            fileSystem = new InterPlanetaryFiles();
            Cid = cid;
            Index = index;
        }

        public string Cid { get; init; }

        public int Index { get; init; }

        public string Content { get; private set; } = string.Empty;

        public uint MemberRank { get; set; }

        public int GroupScore { get; set; }

        public async Task LoadContent()
        {
            if (Content != string.Empty) return;
            Content = await fileSystem.Retrieve(Cid) ?? string.Empty;
        }
    }
}
