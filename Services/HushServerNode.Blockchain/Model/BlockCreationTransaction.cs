namespace HushServerNode.Blockchain.Model;

public class BlockCreationTransaction : TransactionBase
{
    public double Reward { get; set; }

    public BlockCreationTransaction(string blockId, string issuer) : base(
        "8e29c7c1-f2d8-4ff3-9d97-e927e3f40c79",
        blockId,
        issuer)
    {
        this.Reward = 0.5;          // TODO [AboimPinto]: this value is decided by the network. 
    }
}    
