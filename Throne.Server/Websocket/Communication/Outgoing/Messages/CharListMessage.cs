using Throne.Server.Websocket.Communication.Protocol;
using Throne.Server.Websocket.Core.Data;
using Throne.Shared.Communication;

namespace Throne.Server.Websocket.Communication.Outgoing.Messages;

public class CharListMessage : ServerMessage
{
    public CharListMessage(List<CharacterData> chars, int charCount) : base((short)ServerHeaders.CharList)
    {
        PutInt8((sbyte)charCount);
        PutInt8((sbyte)chars.Count);

        foreach (CharacterData character in chars)
        {
            PutInt32(character.Id);
            PutInt32(character.AccountId);
            PutString(character.Name);
            PutString(character.Gender);
        }
    }
}