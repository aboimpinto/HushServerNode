using HushEcosystem.Model.Blockchain;

namespace HushServerNode.Blockchain;

public static class TransactionExtensionMethods
{
    public static VerifiedTransaction GetRewardTransaction(this IEnumerable<VerifiedTransaction> transactions)
    {
        var verifiedRewardTransaction = transactions
            .Where(x => x.SpecificTransaction.TransactionId == BlockCreationTransaction.TypeCode)
            .Single();

        return verifiedRewardTransaction;
    }
}
