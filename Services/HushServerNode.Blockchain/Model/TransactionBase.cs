namespace HushServerNode.Blockchain.Model;

public abstract class TransactionBase
{
    public string TransactionId { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string BlockId { get; set; } = string.Empty;
}