using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using NSubstitute;
using Stratis.SmartContracts;

namespace AlgorithmTests
{
    public class ChainStoreIndexingTests
    {

        [Fact]
        public void MiniMax()
        {
            var answer = new uint[TotalAnswers];
            var score = new int[TotalAnswers];
            for (var n = 0u; n < TotalAnswers; n++)
            {
                answer[n] = n;
                score[n] = GetAnswerScore(n);
            }
            for (var i = 0u; i < TotalAnswers - 1; i++)
            {
                for (var j = i + 1; j > 0; j--)
                {
                    if (score[j - 1] <= score[j]) continue;
                    var higherScore = score[j - 1];
                    var higherScoreAnswer = answer[j - 1];
                    score[j - 1] = score[j];
                    answer[j - 1] = answer[j];
                    score[j] = higherScore;
                    answer[j] = higherScoreAnswer;
                }
            }
            var expected = Scores;
            Array.Sort(expected);
            Assert.Equal(expected, score);
        }

        private static int[] Scores => new[] { 0, -1, 0, 0 };

        private static uint TotalAnswers => (uint)Scores.Length;

        private static int GetAnswerScore(uint index) => Scores[index];

    }

}