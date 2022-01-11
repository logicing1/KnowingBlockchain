using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Fake;

namespace GroupKnowledgeClient.Model
{
    public class Question
    {

        private readonly IFileSystem fileSystem;
        private string name = string.Empty;
        private IList<Answer> answers = new List<Answer>();
        private QuestionContractProxy? contract = default;

        public Question()
        {
            this.fileSystem = new FileSystem();
        }

        public string Contract 
        { 
            get
            {
                return contract != default ? contract.Address : String.Empty; 
            }
            set
            {
                contract = new QuestionContractProxy(value);
            }
        }
        
        public string CID { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        
        public IList<Answer> Answers { get; set; } = new List<Answer>();

        public void Answer(string content)
        {
            throw new NotImplementedException();
        }

        public void Rank() 
        {
            throw new NotImplementedException();
        }

    }
}
