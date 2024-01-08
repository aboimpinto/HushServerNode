using Olimpo;
using HushEcosystem.RpcModel;
using HushEcosystem.RpcModel.CommandDeserializeStrategies;

namespace HushServerNode.ServerService;

public class TcpServerService: ITcpServerService
{
    private readonly IServer _server;

    public TcpServerService(
        IServer server,
        IEnumerable<ICommandDeserializeStrategy> strategies)
    {
        this._server = server;

        this._server.DataReceived.Subscribe(x => 
        {
            var decompressedMessage = x.Message.Decompress();

            var commandStrategy = strategies.SingleOrDefault(x => x.CanHandle(decompressedMessage));
            if (commandStrategy == null)
            {
                throw new InvalidOperationException($"There is no strategy for the command: : {decompressedMessage}");
            }
            
            commandStrategy.Handle(decompressedMessage, x.ChannelId);
        });
    }

    public void SendThroughChannel(string channelId, string message)
    {
        var channel = this._server.ConnectedChannels.OpenChannels
            .Single(x => x.Key == channelId).Value;
        
        channel.Send(message);
    }
}
