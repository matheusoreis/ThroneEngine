namespace Throne.Server.Websocket.Core.Data;

public record AccountData
{
  public int Id;
  public string? Email;
  public string? Password;
  public int CharacterCount;
  public bool IsAdmin;
  public bool IsVip;
  public int Coins;
  public int? CharacterId;
}