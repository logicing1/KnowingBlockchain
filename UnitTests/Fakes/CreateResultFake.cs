using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratis.SmartContracts;

namespace Logicing.Knowing.UnitTests.Fakes
{
    internal class CreateResultFake : ICreateResult
    {
        public Address NewContractAddress { get; } = new Address(42, 5, 5, 5, 5);
        public bool Success { get; } = true;
    }
}
