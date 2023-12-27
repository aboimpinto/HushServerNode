using HushServerNode.ApplicationSettings.Model;
using HushServerNode.Blockchain.Model;

namespace HushServerNode.Blockchain.Builders;

public class BlockBuilder : IBlockBuilder
{
    private string _blockId = string.Empty;
    private string _previousBlockId = string.Empty;
    private string _nextBlockId = string.Empty;
    private double _blockIndex;

    public IBlockBuilder WithBlockId(string blockId)
    {
        this._blockId = blockId;
        return this;
    }
        
    public IBlockBuilder WithPreviousBlockId(string previousBlockId)
    {
        this._previousBlockId = previousBlockId;
        return this;
    }

    public IBlockBuilder WithNextBlockId(string nextBlockId)
    {
        this._nextBlockId = nextBlockId;
        return this;
    }

    public IBlockBuilder WithBlockIndex(double blockIndex)
    {
        this._blockIndex = blockIndex;
        return this;
    }

    public IBlockBuilder WithRewardBeneficiary(StackerInfo stackerInfo)
    {
        return this;
    }

    public Block Build()
    {
        return new Block(
            this._blockId, 
            this._previousBlockId, 
            this._nextBlockId, 
            this._blockIndex);
    }
}
