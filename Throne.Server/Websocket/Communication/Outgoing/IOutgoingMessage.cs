using Throne.Server.Websocket.Communication.Protocol;

namespace Throne.Server.Websocket.Communication.Outgoing;

public interface IOutgoingMessage
{
    protected void DataTo(WsConnection connection, ServerMessage serverMessage);
    protected void DataToAll(ServerMessage message);
    protected void DataToAllExcept(WsConnection exceptConnection, ServerMessage message);
}