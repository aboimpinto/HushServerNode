using HushServerNode.Blockchain.Events;
using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain.Factories;

public class BlockCreatedEventFactory : IBlockCreatedEventFactory
{
    private readonly TransactionBaseConverter _transactionBaseConverter;

    public BlockCreatedEventFactory(TransactionBaseConverter transactionBaseConverter)
    {
        this._transactionBaseConverter = transactionBaseConverter;
    }

    public BlockCreatedEvent GetInstance(Block block)
    {
        return new BlockCreatedEvent(block, this._transactionBaseConverter);
    }
}