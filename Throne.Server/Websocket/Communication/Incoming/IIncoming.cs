using Throne.Server.Websocket.Communication.Protocol;

namespace Throne.Server.Websocket.Communication.Incoming;

public interface IIncoming
{
    public Task Handle(WsConnection connection, ClientMessage clientMessage);
}