using Throne.Server.Communication.Outgoing.Messages;
using Throne.Server.Communication.Protocol;
using Throne.Server.Network;

namespace Throne.Server.Communication.Incoming.Requests;

public class PingRequest : IIncoming
{
  public async Task Handle(WebSocketConnection connection, ClientMessage clientMessage)
  {
    PingMessage message = new();
    await message.SendTo(connection);
  }
}
