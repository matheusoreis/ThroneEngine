using Throne.Server.Communication.Outgoing;
using Throne.Server.Communication.Outgoing.Messages;
using Throne.Server.Communication.Protocol;
using Throne.Server.Network;

namespace Throne.Server.Communication.Incoming.Requests;

public class PingRequest : IIncoming
{
  public void Handle(WebSocketConnection connection, ClientMessage clientMessage)
  {
    PingMessage message = new();
    message.SendTo(connection);
  }
}
