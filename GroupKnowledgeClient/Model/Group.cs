using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Fake;
using Stratis.SmartContracts;

namespace GroupKnowledgeClient.Model
{
    public class Group
    {
        private readonly IFileSystem fileSystem;
        private readonly GroupContractProxy contract;
        private readonly IList<Question> questions;
        private string name = string.Empty;

        public Group(string contractAddress)
        {
            fileSystem = new FileSystem();
            contract = new(contractAddress);
            questions = new List<Question>();
        }

        public string Contract => contract.Address;

        public string Name 
        {
            get
            {
                if (string.IsNullOrEmpty(name)) name = contract.Name();
                return name;
            }
        }

        public ulong Balance { get; set; }

        public IList<Question> Questions
        {
            get
            {
                var questions = new List<Question>();
                var cids = contract.Questions().Split(',');
                foreach (var cid in cids)
                {

                    questions.Add(new Question
                    {
                        CID = cid,
                        Content = fileSystem.Retrieve(cid)
                    });
                }
                return questions;
            }
        }

        public void Join()
        {
            throw new NotImplementedException();
        }

        public void Withdraw()
        {
            throw new NotImplementedException();
        }

        public void Ask(string content)
        {
            var cid = fileSystem.Store(content);
            contract.Ask(cid);
        }
    }
}
