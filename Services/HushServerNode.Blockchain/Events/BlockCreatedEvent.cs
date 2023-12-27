using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain.Events
{
    public class BlockCreatedEvent
    {
        public Block Block { get; }

        public BlockCreatedEvent(Block block)
        {
            this.Block = block;
        }
    }
}