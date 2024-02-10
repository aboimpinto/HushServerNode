using HushEcosystem.Model;
using HushEcosystem.Model.Blockchain;
using HushServerNode.ApplicationSettings.Model;

namespace HushServerNode.Blockchain.Builders;

public class BlockBuilder : IBlockBuilder
{
    private string _blockId = string.Empty;
    private string _previousBlockId = string.Empty;
    private string _nextBlockId = string.Empty;
    private double _blockIndex;
    private TransactionBase _rewardTransaction;
    private VerifiedTransaction _verifiedRewardTransaction;
    private StackerInfo _stackerInfo;
    private readonly TransactionBaseConverter _transactionBaseConverter;

    public BlockBuilder(TransactionBaseConverter transactionBaseConverter)
    {
        this._transactionBaseConverter = transactionBaseConverter;
    }

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

    public IBlockBuilder WithRewardBeneficiary(StackerInfo stackerInfo, double blockHeight)
    {
        this._rewardTransaction = new BlockCreationTransactionBuilder()
            .WithIssuerAddress(stackerInfo.PublicSigningAddress)
            .Build();

        this._rewardTransaction.HashObject(this._transactionBaseConverter);
        this._rewardTransaction.Sign(stackerInfo.PrivateSigningKey, this._transactionBaseConverter);
        
        this._verifiedRewardTransaction = new VerifiedTransaction
        {
            SpecificTransaction = this._rewardTransaction,
            ValidatorAddress = stackerInfo.PublicSigningAddress
        };

        // this._verifiedRewardTransaction.HashObject(this._transactionBaseConverter);
        // this._verifiedRewardTransaction.Sign(stackerInfo.PrivateSigningKey, this._transactionBaseConverter);

        this._stackerInfo = stackerInfo;

        return this;
    }

    public Block Build()
    {
        var block = new Block(
            this._blockId, 
            this._previousBlockId, 
            this._nextBlockId, 
            this._blockIndex);

        // Add the verified reward transaction
        this._verifiedRewardTransaction.BlockIndex = this._blockIndex;
        this._verifiedRewardTransaction.HashObject(this._transactionBaseConverter);
        this._verifiedRewardTransaction.Sign(this._stackerInfo.PrivateSigningKey, this._transactionBaseConverter);
        block.Transactions.Add(this._verifiedRewardTransaction);

        // Get validated transactions from the MemPool
        // TODO [AboimPinto]: Add MemPool transactions to the block

        block.FinalizeBlock();
        block.Sign(this._stackerInfo.PrivateSigningKey, this._transactionBaseConverter);

        return block;
    }
}
