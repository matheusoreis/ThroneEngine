using Throne.Server.Websocket.Communication.Protocol;

namespace Throne.Server.Websocket.Communication.Outgoing.Messages;

public class CharDeletedMessage : ServerMessage
{
    public CharDeletedMessage(int charId) : base((short)ServerHeaders.CharDeleted)
    {
        PutInt32(charId);
    }
}