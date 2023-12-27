namespace HushServerNode.Blockchain.Model;

public class BlockCreationTransaction : TransactionBase
{
    public string DestinationAddress { get; set; } = string.Empty;

    public double Reward { get; set; }
}
