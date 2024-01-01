using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain;

public static class TransactionExtensionMethods
{
    public static BlockCreationTransaction GetRewardTransaction(this IEnumerable<TransactionBase> transactions)
    {
        var transaction = transactions
            .Single(x => x.Type == "8e29c7c1-f2d8-4ff3-9d97-e927e3f40c79");

        return (BlockCreationTransaction)transaction;
    }
}
