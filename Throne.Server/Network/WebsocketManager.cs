using System.Net.WebSockets;
using System.Reflection.Metadata;
using Throne.Server.Communication.Outgoing.Messages;
using Throne.Server.Core.Memory;
using Throne.Shared.Logger;
using Throne.Shared.Slots;

namespace Throne.Server.Network;

public class WebsocketManager
{
  private readonly Slots<WebSocketConnection> connections;

  public WebsocketManager()
  {
    connections = MemoryManager.Instance.Connections;
  }

  public async Task WebSocketOpen(WebSocket webSocket, string ip)
  {
    int? connectionId = connections.GetFirstEmptySlot();

    if (connectionId == null)
    {
      await HandleFullServer(webSocket, ip);
      return;
    }

    var connection = new WebSocketConnection(webSocket, connectionId.Value, ip);
    connections.Add(connection);
    Logger.Info($"New connection from: {ip}");
  }

  public async Task WebSocketMessage(WebSocket webSocket, byte[] message)
  {
    var connection = GetConnectionBySocket(webSocket);

    if (connection != null)
    {
      await connection.ProcessMessage(message);
    }
    else
    {
      Logger.Error("Connection not found for WebSocket.");
      await CleanupConnection(webSocket);
    }
  }

  public async Task WebSocketClose(WebSocket webSocket)
  {
    var connection = GetConnectionBySocket(webSocket);

    if (connection != null)
    {
      Logger.Info($"Connection closed, address: {connection.Ip}");
      await CleanupConnection(webSocket);
    }
  }

  private async Task HandleFullServer(WebSocket webSocket, string ip)
  {
    Logger.Info($"Server is full, disconnecting client: {ip}");

    WebSocketConnection? webSocketConnection = new(webSocket, -1, ip);

    AlertData alertData = new()
    {
      Type = AlertType.Info,
      Message = "Server is full! disconnecting..."
    };

    AlertMessage alertMessage = new(alertData);

    await alertMessage.SendTo(webSocketConnection);
    await CleanupConnection(webSocket);
  }

  private async Task CleanupConnection(WebSocket webSocket)
  {
    var connection = GetConnectionBySocket(webSocket);

    if (connection != null)
    {
      connections.Remove(connection.Id);
      Logger.Info($"Connection removed, address: {connection.Id}");

      if (connection.IsOpen())
      {
        await connection.Close();
      }
    }
  }

  private WebSocketConnection? GetConnectionBySocket(WebSocket webSocket)
  {
    foreach (var index in connections.GetFilledSlots())
    {
      var connection = connections.Get(index);

      if (connection?.WebSocket == webSocket)
      {
        return connection;
      }
    }
    return null;
  }
}
