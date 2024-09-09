namespace Throne.Server.Websocket.Core.Data;

public struct CharacterData
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Gender { get; set; }
}