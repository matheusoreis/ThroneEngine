using Throne.Server.Websocket.Communication.Protocol;
using Throne.Shared.Communication;

namespace Throne.Server.Websocket.Communication.Outgoing.Messages;

public class SignInMessage : ServerMessage
{
    public SignInMessage(int accountId) : base((short)ServerHeaders.SignInSuccess)
    {
        PutInt32(accountId);
    }
}