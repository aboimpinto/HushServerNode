using HushEcosystem;
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
    IHandle<TransationsWithAddressRequestedEvent>
{
    private readonly ITcpServerService _tcpServerService;
    private readonly IBlockchainService _blockchainService;
    private readonly IEventAggregator _eventAggregator;

    public Rpc(
        ITcpServerService tcpServerService,
        IBlockchainService blockchainService,
        IEventAggregator eventAggregator)
    {
        this._tcpServerService = tcpServerService;
        this._blockchainService = blockchainService;
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
            .SendThroughChannel(message.ChannelId, handshakeResponse.ToJson().Compress());
    }

    public void Handle(TransationsWithAddressRequestedEvent message)
    {
        var transactions = this._blockchainService
            .ListTransactionsForAddress(message.TransationsWithAddressRequest.Address, message.TransationsWithAddressRequest.LastHeightSynched);

        var transactionsWithAddressResponse = new TransactionsWithAddressResponseBuilder()
            .Build();
    }
}
