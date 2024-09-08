namespace Throne.Server.Websocket.Core.Data;

public record AccountData
{
    public int CharacterCount;
    public int? CharacterId;
    public int Coins;
    public string? Email;
    public int Id;
    public bool IsAdmin;
    public bool IsVip;
    public string? Password;
}