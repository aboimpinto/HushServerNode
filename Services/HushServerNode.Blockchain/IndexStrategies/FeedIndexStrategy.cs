using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HushEcosystem.Model.Blockchain;
using HushEcosystem.Model.Builders;
using HushEcosystem.Model.Rpc.Feeds;

namespace HushServerNode.Blockchain.IndexStrategies;

public class FeedIndexStrategy : IIndexStrategy
{
    private readonly IBlockchainIndexDb _blockchainIndexDb;

    public FeedIndexStrategy(IBlockchainIndexDb blockchainIndexDb)
    {
        this._blockchainIndexDb = blockchainIndexDb;
    }

    public bool CanHandle(VerifiedTransaction verifiedTransaction)
    {
        if (verifiedTransaction.SpecificTransaction.Id == Feed.TypeCode)
        {
            return true;
        }
 
        return false;
    }

    public Task Handle(VerifiedTransaction verifiedTransaction)
    {
        var feed = (Feed)verifiedTransaction.SpecificTransaction;

        switch(feed.FeedType)
        {
            case FeedTypeEnum.Personal:
                this.HandlesPersonalFeed(feed, verifiedTransaction.BlockIndex);
                break;
        }

        return Task.CompletedTask;
    }

    private void HandlesPersonalFeed(Feed feed, double blockIndex)
    {
        var feedParticipantProfile = this._blockchainIndexDb.Profiles.SingleOrDefault(x => x.Issuer == feed.Issuer);
        var feedName = string.Empty;
        if (feedParticipantProfile == null)
        {
            feedName = feed.FeedParticipantPublicAddress.Substring(0, 10);
        }
        else
        {
            feedName = $"{feedParticipantProfile.UserName} (You)";
        }

        // Check if there is already a Personal Feed for the user
        
        var hasPersonalFeed = this._blockchainIndexDb.Feeds.Values
            .SelectMany(x => x)
            .Where(x => x.FeedType == FeedTypeEnum.Personal)
            .Any();

        if (hasPersonalFeed)
        {
            var personalFeed = this._blockchainIndexDb.Feeds[feed.FeedParticipantPublicAddress]
                .Single(x => x.FeedType == FeedTypeEnum.Personal);
            
            personalFeed.FeedTitle = feedName;
            personalFeed.BlockIndex = blockIndex;
        }
        else
        {
            var newPersonalFeed = new FeedDefinitionBuilder()
                .WithFeedId(feed.FeedId)
                .WithFeedType(feed.FeedType)
                .WithParticipantAddress(feed.FeedParticipantPublicAddress)
                .WithFeedTitle(feedName)
                .WithBlockIndex(blockIndex)
                .Build();

            this._blockchainIndexDb.Feeds.Add(feed.FeedParticipantPublicAddress, new List<FeedDefinition> { newPersonalFeed });
        }


        

        // // checks if the user has the feed 
        //  if (this._blockchainIndexDb.Feeds.ContainsKey(feed.FeedParticipantPublicAddress))
        // {
        //     // check if there is a personal feed already for the user
        //     var hasPersonalFeed = this._blockchainIndexDb.Feeds[feed.FeedParticipantPublicAddress]
        //         .Any(x => x.FeedType == FeedTypeEnum.Personal && x.FeedParticipant == feed.Issuer);

        //     if (hasPersonalFeed)
        //     {
        //         // Personal feed already exist
        //     }
        //     else
        //     {
        //         var newPersonalFeed = new FeedDefinitionBuilder()
        //                 .WithFeedId(feed.FeedId)
        //                 .WithFeedType(feed.FeedType)
        //                 .WithParticipantAddress(feed.FeedParticipantPublicAddress)
        //                 .WithFeedTitle(feedName)
        //                 .WithBlockIndex(blockIndex)
        //                 .Build();

        //         this._blockchainIndexDb.Feeds[feed.FeedParticipantPublicAddress]
        //             .Add(newPersonalFeed);
        //     }
        // }
        // else
        // {

        // }

    }
}
