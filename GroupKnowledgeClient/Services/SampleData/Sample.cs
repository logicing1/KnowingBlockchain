using System.Diagnostics;
using System.Formats.Asn1;
using System.Security.Cryptography;
using System.Text;
using GroupKnowledgeClient.Model;
using Stratis.SmartContracts;

namespace GroupKnowledgeClient.Services.SampleData
{
    public class Sample
    {
        private readonly Random random;

        private readonly string[] groupNames = new[]
        {
            "Cross-Enterprise Manufacturing",
            "WebAssembly Programing",
            "Quantum Field Theory",
            "Sumerian Antiquities",
            "Particle Swarm Optimization",
        };

        public Sample(int seed = 55)
        {
            random = new Random(seed);
            Groups = GenerateGroups();
        }

        public IDictionary<string, GroupData> Groups { get; private set; }

        private static string Hash(string text)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToHexString(hash).ToLower();
        }

        private IDictionary<string, GroupData> GenerateGroups()
        {
            var groups = new List<GroupData>();
            for (var index = 0u; index < groupNames.Length; index++)
            {
                groups.Add(new GroupData
                {
                    Address = new Address(0, 0, 0, 0, index).ToString(),
                    Name = groupNames[index],
                    Balance = index % 2 == 0 ? 0u : 100000u,
                    Questions = GenerateQuestions(index)
                });
                Debug.WriteLine("stop");
            }
            return groups.ToDictionary(g => g.Address, g => g);
        }

        private List<QuestionData> GenerateQuestions(uint groupIndex)
        {
            const int Min = 2;
            const int Max = 8;
            const string Text =
                "Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?";

            var questions = new List<QuestionData>();
            for (var index = 0u; index < random.Next(Min, Max); index++)
            {
                var content = Text.Substring((int)index * 5, Text.Length - ((int)index * 10));
                questions.Add(new QuestionData
                {
                    Address = new Address(0, 0, 0, index, groupIndex).ToString(),
                    CID = Hash(content),
                    Content = content,
                    Answers = GenerateAnswers()

                });
            }
            return questions;

        }

        private List<AnswerData> GenerateAnswers()
        {
            const int Min = 2;
            const int Max = 8;
            const string Text =
                "But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete account of the system, and expound the actual teachings of the great explorer of the truth, the master-builder of human happiness. No one rejects, dislikes, or avoids pleasure itself, because it is pleasure, but because those who do not know how to pursue pleasure rationally encounter consequences that are extremely painful. Nor again is there anyone who loves or pursues or desires to obtain pain of itself, because it is pain, but because occasionally circumstances occur in which toil and pain can procure him some great pleasure. To take a trivial example, which of us ever undertakes laborious physical exercise, except to obtain some advantage from it? But who has any right to find fault with a man who chooses to enjoy a pleasure that has no annoying consequences, or one who avoids a pain that produces no resultant pleasure.";

            var answers = new List<AnswerData>();
            var totalAnswers = random.Next(Min, Max);
            for (var index = 0u; index < totalAnswers; index++)
            {
                var content = Text.Substring((int)index * 5, Text.Length - ((int)index * 10));
                answers.Add(new AnswerData
                {
                    CID = Hash(content),
                    Content = content,
                    VoterRank = (uint)random.Next(0, totalAnswers)
                });
            }
            return answers;
        }

        public struct GroupData
        {
            public string Address;
            public string Name;
            public ulong Balance;
            public IList<QuestionData> Questions;
        }

        public struct QuestionData
        {
            public string Address;
            public string CID;
            public string Content;
            public IList<AnswerData> Answers;
        }

        public struct AnswerData
        {
            public string CID;
            public string Content;
            public int GroupScore;
            public uint VoterRank;
        }
    }
}
