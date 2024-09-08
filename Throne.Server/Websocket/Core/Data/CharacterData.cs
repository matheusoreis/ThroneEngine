namespace Throne.Server.Websocket.Core.Data;

public struct CharacterData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public int GenderId { get; set; }
    public int CharCount { get; set; }
}