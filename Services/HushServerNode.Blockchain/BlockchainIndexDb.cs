using System.Collections.Generic;
using HushEcosystem.Model.Blockchain;
using HushEcosystem.Model.Rpc.Feeds;

namespace HushServerNode.Blockchain;

public class BlockchainIndexDb : IBlockchainIndexDb
{
    public IDictionary<string, double> AddressBalance { get; set; }

    public IDictionary<string, List<VerifiedTransaction>> GroupedTransactions { get; set; }

    public IList<UserProfile> Profiles { get; set; }
    
    public IDictionary<string, List<FeedDefinition>> Feeds { get; set; }

    public IDictionary<string, List<FeedMessageDefinition>> FeedMessages { get; set; }

    public BlockchainIndexDb()
    {
        this.AddressBalance = new Dictionary<string, double>();
        this.GroupedTransactions = new Dictionary<string, List<VerifiedTransaction>>();
        this.Profiles = new List<UserProfile>();
        this.Feeds = new Dictionary<string, List<FeedDefinition>>();
        this.FeedMessages = new Dictionary<string, List<FeedMessageDefinition>>();
    }
}
