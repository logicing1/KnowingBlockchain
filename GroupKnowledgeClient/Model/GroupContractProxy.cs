using GroupKnowledgeClient.Services;
using GroupKnowledgeClient.Services.Fake;
using System.Security.Cryptography;
using System.Text;

namespace GroupKnowledgeClient.Model
{
    public class GroupContractProxy
    {
        private IBlockchain blockchain;

        public GroupContractProxy(string address)
        {
            this.Address = address;
            blockchain = new Blockchain();
        }

        public string Address { get; }

        public string Name()
        {
            return "Spanish Poultry";
        }

        public string Questions()
        {
            string[] content = new[]
            {
                "Why did the chicken cross the road?",
                "Where does the rain in Spain mainly fall?"
            };
            string[] cids = new string[content.Length];
            for (int i = 0; i < content.Length; i++)
            {
                using var hash = SHA256.Create();
                var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(content[i]));
                cids[i] = Convert.ToHexString(byteArray).ToLower(); 
            }
            return string.Join(',', cids);
        }

        public void Ask(string cid)
        {
            return;
        }

        public void Answer(string questionCid, string answerCid)
        {
            throw new NotImplementedException();
        }

    }
}