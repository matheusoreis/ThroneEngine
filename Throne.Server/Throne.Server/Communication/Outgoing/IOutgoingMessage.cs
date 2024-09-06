using Throne.Server.Communication.Protocol;
using Throne.Server.Network;

namespace Throne.Server.Communication.Outgoing;

public interface IOutgoingMessage
{
  protected void DataTo(WebSocketConnection connection, ServerMessage serverMessage);
  protected void DataToAll(ServerMessage message);
  protected void DataToAllExcept(WebSocketConnection exceptConnection, ServerMessage message);
}