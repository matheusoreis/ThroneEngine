using Throne.Server.Websocket.Communication.Protocol;
using Throne.Shared.Communication;

namespace Throne.Server.Websocket.Communication.Outgoing.Messages;

public class PingMessage() : ServerMessage((short)ServerHeaders.Pong)
{
}