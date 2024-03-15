using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HushEcosystem.Model.Blockchain;
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

        if (this._blockchainIndexDb.Feeds.ContainsKey(feed.FeedParticipantPublicAddress))
        {
            // get the profile of the participant 
            var feedParticipantProfile = this._blockchainIndexDb.Profiles.SingleOrDefault(x => x.Issuer == feed.Issuer);
            var profileName = string.Empty;

            if (feedParticipantProfile != null)
            {
                profileName = feedParticipantProfile.UserName;
            }

            if (feed.FeedType == FeedTypeEnum.Personal)
            {
                // check if there is a personal feed already for the user
                var hasPersonalFeed = this._blockchainIndexDb.Feeds[feed.FeedParticipantPublicAddress]
                    .Any(x => x.FeedType == FeedTypeEnum.Personal && x.FeedParticipant == feed.Issuer);

                if (hasPersonalFeed)
                {
                    // Personal feed already exist
                }
                else
                {
                    this._blockchainIndexDb.Feeds[feed.FeedParticipantPublicAddress]
                        .Add(new FeedDefinition
                        {
                            FeedId = feed.FeedId,
                            FeedType = feed.FeedType,
                            FeedParticipant = feed.FeedParticipantPublicAddress,
                            FeedTitle = $"{profileName} (You)",
                            BlockIndex = verifiedTransaction.BlockIndex
                        });
                }
            }

            if (feed.Issuer == feed.FeedParticipantPublicAddress)
            {
                // the issuer is the feed participant
            }
        }
        else
        {
            this._blockchainIndexDb.Feeds.Add(
                feed.FeedParticipantPublicAddress, 
                new List<FeedDefinition>
                {
                    new FeedDefinition
                    {
                        FeedId = feed.FeedId,
                        FeedType = feed.FeedType,
                        FeedParticipant = feed.FeedParticipantPublicAddress,
                        FeedTitle = feed.FeedParticipantPublicAddress,
                        BlockIndex = verifiedTransaction.BlockIndex
                    }
                });
        }

        return Task.CompletedTask;
    }
}
