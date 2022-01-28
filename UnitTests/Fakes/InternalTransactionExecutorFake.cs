using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratis.SmartContracts;

namespace Logicing.Knowing.UnitTests.Fakes
{
    internal class InternalTransactionExecutorFake : IInternalTransactionExecutor
    {
        public ITransferResult Transfer(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer)
        {
            return new TransferResultFake();
        }

        public ITransferResult Call(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer, string methodName, object[] parameters, ulong gasLimit = 0)
        {
            throw new NotImplementedException();
        }

        public ICreateResult Create<T>(ISmartContractState smartContractState, ulong amountToTransfer, object[] parameters, ulong gasLimit = 0)
        {
            return new CreateResultFake();
        }
    }
}
