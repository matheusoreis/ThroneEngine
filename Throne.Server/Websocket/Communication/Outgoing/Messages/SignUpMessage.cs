using Throne.Server.Websocket.Communication.Protocol;
using Throne.Shared.Communication;

namespace Throne.Server.Websocket.Communication.Outgoing.Messages;

public class SignUpMessage() : ServerMessage((short)ServerHeaders.SignUpSuccess);