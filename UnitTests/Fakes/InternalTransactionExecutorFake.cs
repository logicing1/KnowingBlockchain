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
        private object transferResult;

        public InternalTransactionExecutorFake(object transferResult = null)
        {
            this.transferResult = transferResult ?? true;
        }

        public ITransferResult Transfer(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer)
        {
            return new TransferResultFake(transferResult);
        }

        public ITransferResult Call(ISmartContractState smartContractState, Address addressTo, ulong amountToTransfer, string methodName, object[] parameters, ulong gasLimit = 0)
        {
            return new TransferResultFake(transferResult);
        }

        public ICreateResult Create<T>(ISmartContractState smartContractState, ulong amountToTransfer, object[] parameters, ulong gasLimit = 0)
        {
            return new CreateResultFake();
        }
    }
}
