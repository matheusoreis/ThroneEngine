using Throne.Server.Websocket.Communication.Protocol;

namespace Throne.Server.Websocket.Communication.Outgoing;

public interface IOutgoingMessage
{
    protected void DataTo(WSConnection connection, ServerMessage serverMessage);
    protected void DataToAll(ServerMessage message);
    protected void DataToAllExcept(WSConnection exceptConnection, ServerMessage message);
}