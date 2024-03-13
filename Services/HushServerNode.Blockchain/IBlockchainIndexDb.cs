using System.Collections.Generic;
using HushEcosystem.Model.Blockchain;

namespace HushServerNode.Blockchain;

public interface IBlockchainIndexDb
{
    Dictionary<string, double> AddressBalance { get; set; }

    Dictionary<string, List<VerifiedTransaction>> GroupedTransactions { get; set; }

    IList<UserProfile> Profiles { get; set; }
}
