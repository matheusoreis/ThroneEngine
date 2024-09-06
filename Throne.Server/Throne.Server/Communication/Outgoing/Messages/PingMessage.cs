using Throne.Server.Communication.Protocol;
using Throne.Shared.Communication;

namespace Throne.Server.Communication.Outgoing.Messages;

public class PingMessage : ServerMessage
{
  public PingMessage() : base((short)ServerHeaders.Pong) { }
}
