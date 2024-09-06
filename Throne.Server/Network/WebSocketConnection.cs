using System.Net.WebSockets;
using Throne.Server.Communication.Protocol;
using Throne.Shared.Logger;

namespace Throne.Server.Network;

public class WebSocketConnection(WebSocket webSocket, int id, string ip)
{
  public WebSocket WebSocket { get; } = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
  public int Id { get; } = id;

  public string Ip { get; } = ip;
  private MessageHandler Handler { get; } = new();

  public async Task Close()
  {
    if (WebSocket.State == WebSocketState.Open || WebSocket.State == WebSocketState.CloseSent)
    {
      try
      {
        await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
        Logger.Info($"Connection with ID {Id} closed successfully.");
      }
      catch (Exception ex)
      {
        Logger.Error($"Error closing connection with ID {Id}: {ex.Message}");
      }
    }
    else
    {
      Logger.Error($"Attempted to close connection with ID {Id}, but WebSocket is not open.");
    }
  }

  public bool IsOpen() => WebSocket.State == WebSocketState.Open;

  public async Task ProcessMessage(byte[] buffer)
  {
    var message = new ClientMessage(buffer);
    await Handler.ProcessMessage(this, message);
  }
}
