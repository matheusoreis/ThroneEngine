using Throne.Server.Communication.Protocol;
using Throne.Server.Core.Memory;
using Throne.Shared.Communication;

namespace Throne.Server.Communication.Outgoing.Messages;

public class PingMessage() : ServerMessage((short)ServerHeaders.Pong)
{
}
