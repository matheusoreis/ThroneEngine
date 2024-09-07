using Throne.Server.Websocket.Communication.Protocol;

namespace Throne.Server.Websocket.Communication.Incoming;

public interface IIncoming
{
  public Task Handle(WSConnection connection, ClientMessage clientMessage);

}
