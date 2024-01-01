using HushServerNode.Blockchain.Events;
using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain.Factories;

public interface IBlockCreatedEventFactory
{
    BlockCreatedEvent GetInstance(Block block);
}
