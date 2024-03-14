using System.Collections.Generic;
using HushEcosystem.Model.Blockchain;
using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain;

public interface IBlockchainIndexDb
{
    Dictionary<string, double> AddressBalance { get; set; }

    Dictionary<string, List<VerifiedTransaction>> GroupedTransactions { get; set; }

    IList<UserProfile> Profiles { get; set; }

    Dictionary<string, List<FeedDefinition>> Feeds { get; set; }
}
