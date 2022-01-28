using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChainTests
{
    public class TestChain1 : ITestChain
    {

        public string Location { get; } = @"http://localhost:38223";

        public long DefaultGasPrice { get; } = 100;

        public long DefaultGasLimit { get; } = 200000;

        public int TransactionTime { get; } = 20000;
    }
}
