namespace HushServerNode.Blockchain.Model;

public interface ITransaction
{
    string TransactionId { get; }

    string Type { get; }

    string BlockId { get; }

    string Issuer { get; }

    string Signature { get; }
}
