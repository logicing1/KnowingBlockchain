using System;
using Stratis.SmartContracts;
using Stratis.SmartContracts.RuntimeObserver;

namespace Logicing.Knowing.UnitTests.Fakes
{
    public class SmartContractStateFake : ISmartContractState
    {
        private readonly Func<IMessage> messageSource;
        private readonly Func<IBlock> blockSource;
        private ulong balance;

        public SmartContractStateFake()
        {
            PersistentState = new PersistantStateFake();
            blockSource = () => new BlockFake(1);
            messageSource = () => new MessageFake(1);
        }

        public SmartContractStateFake(Func<IMessage> messageSource, Func<IBlock> blockSource)
        {
            PersistentState = new PersistantStateFake();
            this.messageSource = messageSource;
            this.blockSource = blockSource;
        }

        public IBlock Block => blockSource.Invoke();

        public IMessage Message => ReadMessage();

        public IPersistentState PersistentState { get; private set; }

        public IGasMeter GasMeter { get; }

        public IContractLogger ContractLogger { get; }

        public IInternalTransactionExecutor InternalTransactionExecutor => new InternalTransactionExecutorFake();

        public IInternalHashHelper InternalHashHelper { get; }

        public ISerializer Serializer { get; }

        public Func<ulong> GetBalance => GetCurrentBalance;

        private ulong GetCurrentBalance() => balance;

        private IMessage ReadMessage()
        {
            var message = messageSource.Invoke();
            balance += message.Value;
            return message;
        }

    }
}