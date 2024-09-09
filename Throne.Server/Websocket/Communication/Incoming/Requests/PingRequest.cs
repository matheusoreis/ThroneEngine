using Throne.Server.Websocket.Communication.Outgoing.Messages;
using Throne.Server.Websocket.Communication.Protocol;

namespace Throne.Server.Websocket.Communication.Incoming.Requests;

public class PingRequest : IIncoming
{
    public async Task Handle(WsConnection connection, ClientMessage clientMessage)
    {
        PingMessage message = new();
        await message.SendTo(connection);
    }
}