using HushEcosystem.RpcModel;
using HushEcosystem.RpcModel.GlobalEvents;
using HushEcosystem.RpcModel.Handshake;
using HushServerNode.ServerService;
using Olimpo;

namespace HushServerNode.RpcManager;

public class Rpc :
    IRpc,
    IHandle<HandshakeRequestedEvent>
{
    private readonly ITcpServerService _tcpServerService;
    private readonly IEventAggregator _eventAggregator;

    public Rpc(
        ITcpServerService tcpServerService,
        IEventAggregator eventAggregator)
    {
        this._tcpServerService = tcpServerService;
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
}
