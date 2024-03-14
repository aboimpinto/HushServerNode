using System.Collections.Generic;
using HushEcosystem.Model.Blockchain;
using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain;

public class BlockchainIndexDb : IBlockchainIndexDb
{
    public Dictionary<string, double> AddressBalance { get; set; }
    public Dictionary<string, List<VerifiedTransaction>> GroupedTransactions { get; set; }
    public IList<UserProfile> Profiles { get; set; }
    public Dictionary<string, List<FeedDefinition>> Feeds { get; set; }

    public BlockchainIndexDb()
    {
        this.AddressBalance = new Dictionary<string, double>();
        this.GroupedTransactions = new Dictionary<string, List<VerifiedTransaction>>();
        this.Profiles = new List<UserProfile>();
        this.Feeds = new Dictionary<string, List<FeedDefinition>>();
    }
}
