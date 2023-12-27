using HushServerNode.ApplicationSettings.Model;
using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain.Builders;

public interface IBlockBuilder
{
    IBlockBuilder WithBlockId(string blockId);

    IBlockBuilder WithPreviousBlockId(string previousBlockId);

    IBlockBuilder WithNextBlockId(string nextBlockId);

    IBlockBuilder WithBlockIndex(double blockIndex);

    IBlockBuilder WithRewardBeneficiary(StackerInfo stackerInfo);

    Block Build();
}