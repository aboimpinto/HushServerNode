using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain.TransactionConvertersStrategies;

public interface ISpecificTransactionDeserializer
{
    bool CanHandle(string transactionType);

    TransactionBase Handle(string rawText);
}