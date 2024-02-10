using HushEcosystem;
using HushEcosystem.Model;
using HushEcosystem.Model.Rpc.Blockchain;
using HushEcosystem.Model.Rpc.GlobalEvents;
using HushEcosystem.Model.Rpc.Handshake;
using HushEcosystem.Model.Rpc.Transactions;
using HushServerNode.Blockchain;
using HushServerNode.ServerService;
using Olimpo;

namespace HushServerNode.RpcManager;

public class Rpc :
    IRpc,
    IHandle<HandshakeRequestedEvent>,
    IHandle<BlockchainHeightRequestedEvent>,
    IHandle<TransactionsWithAddressRequestedEvent>,
    IHandle<BalanceByAddressRequestedEvent>
{
    private readonly ITcpServerService _tcpServerService;
    private readonly IBlockchainService _blockchainService;
    private readonly TransactionBaseConverter _transactionBaseConverter;
    private readonly IEventAggregator _eventAggregator;

    public Rpc(
        ITcpServerService tcpServerService,
        IBlockchainService blockchainService,
        TransactionBaseConverter transactionBaseConverter,
        IEventAggregator eventAggregator)
    {
        this._tcpServerService = tcpServerService;
        this._blockchainService = blockchainService;
        this._transactionBaseConverter = transactionBaseConverter;
        this._eventAggregator = eventAggregator;

        this._eventAggregator.Subscribe(this);
    }

    public void Handle(HandshakeRequestedEvent message)
    {
        // TODO [AboimPinto] Implement the rules that will accept the Handshake Request or not.

        var handshakeResponse = new HandshakeResponseBuilder()
            .WithResult(true)
            .WithFailureReason(string.Empty)
            .Build();

        this._tcpServerService
            .SendThroughChannel(message.ChannelId, handshakeResponse.ToJson(this._transactionBaseConverter).Compress());
    }

    public void Handle(BlockchainHeightRequestedEvent message)
    {
        var height =  this._blockchainService.CurrentBlockIndex;

        var blockchainHeightResponse = new BlockchainHeightResponse
        { 
            Height = height 
        };

        this._tcpServerService
            .SendThroughChannel(message.ChannelId, blockchainHeightResponse.ToJson(this._transactionBaseConverter).Compress());
    }

    public void Handle(TransactionsWithAddressRequestedEvent message)
    {
        var transactions = this._blockchainService
            .ListTransactionsForAddress(message.TransationsWithAddressRequest.Address, message.TransationsWithAddressRequest.LastHeightSynched);

        var transactionsWithAddressResponse = new TransactionsWithAddressResponseBuilder()
            .WithTransactions(transactions)
            .Build();

        this._tcpServerService
            .SendThroughChannel(
                message.ChannelId, 
                transactionsWithAddressResponse
                    .ToJson(this._transactionBaseConverter)
                    .Compress());
    }

    public void Handle(BalanceByAddressRequestedEvent message)
    {
        var balance = this._blockchainService
            .GetBalanceForAddress(message.BalanceByAddressRequest.Address);

        var balanceByAddressResponse = new BalanceByAddressResponseBuilder()
            .WithBalance(balance)
            .Build();

        this._tcpServerService
            .SendThroughChannel(message.ChannelId, balanceByAddressResponse.ToJson(this._transactionBaseConverter).Compress());
    }
}
