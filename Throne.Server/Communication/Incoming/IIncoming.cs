using Throne.Server.Communication.Protocol;
using Throne.Server.Network;

namespace Throne.Server.Communication.Incoming;

public interface IIncoming
{
  public Task Handle(WebSocketConnection connection, ClientMessage clientMessage);

}
