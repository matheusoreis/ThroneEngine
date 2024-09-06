using System.Net.WebSockets;
using Throne.Server.Communication.Outgoing.Messages;
using Throne.Server.Core.Memory;
using Throne.Shared.Logger;
using Throne.Shared.Slots;

namespace Throne.Server.Network;

public class WebsocketManager(MemoryManager memoryManager)
{
  private readonly Slots<WebSocketConnection> connections = memoryManager.Connections;

  public async Task HandleWebSocketConnection(HttpListenerWebSocketContext wsContext, string ip)
  {
    WebSocket webSocket = wsContext.WebSocket;
    var receivedData = new List<byte>();

    try
    {
      await WebSocketOpen(webSocket, ip);

      var buffer = new byte[1024 * 4];
      while (webSocket.State == WebSocketState.Open)
      {
        var segment = new ArraySegment<byte>(buffer);
        WebSocketReceiveResult result = await webSocket.ReceiveAsync(segment, CancellationToken.None);

        receivedData.AddRange(buffer.Take(result.Count));

        if (result.EndOfMessage)
        {
          await WebSocketMessage(webSocket, receivedData.ToArray());
          receivedData.Clear();
        }

        if (result.MessageType == WebSocketMessageType.Close)
        {
          await WebSocketClose(webSocket);
          break;
        }
      }
    }
    catch (Exception)
    {
      await WebSocketClose(webSocket);
    }
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

    if (connection == null)
    {
      Logger.Error("Connection not found for WebSocket.");
      await CleanupConnection(webSocket);
      return;
    }

    try
    {
      await connection.ProcessMessage(message);
    }
    catch (Exception ex)
    {
      Logger.Error($"Error processing message: {ex.Message}");
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

    WebSocketConnection webSocketConnection = new(webSocket, -1, ip);

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
