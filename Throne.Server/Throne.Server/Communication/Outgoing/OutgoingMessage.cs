using System.Net.WebSockets;
using Throne.Server.Communication.Protocol;
using Throne.Server.Core.Memory;
using Throne.Server.Network;
using Throne.Shared.Logger;
using Throne.Shared.Slots;

namespace Throne.Server.Communication.Outgoing;

public abstract class OutgoingMessage
{
  private readonly Slots<WebSocketConnection> connections;

  protected OutgoingMessage()
  {
    connections = MemoryManager.Instance.Connections;
  }

  protected static void DataTo(WebSocketConnection connection, ServerMessage serverMessage)
  {
    try
    {
      WebSocketMessageType webSocketType = WebSocketMessageType.Binary;
      CancellationToken tokenType = CancellationToken.None;
      connection.WebSocket.SendAsync(serverMessage.GetBuffer(), webSocketType, true, tokenType).Wait();
    }
    catch (Exception e)
    {
      Logger.Error($"Error sending data to the client! Error: {e.Message}");
    }
  }

  protected void DataToAll(ServerMessage message)
  {
    foreach (var index in connections.GetFilledSlots())
    {
      var connection = connections.Get(index);

      if (connection?.WebSocket != null)
      {
        try
        {
          DataTo(connection, message);
        }
        catch (Exception ex)
        {
          Logger.Error("Error sending data to the client! Error: " + ex.Message);
        }
      }
    }
  }

  protected void DataToAllExcept(WebSocketConnection exceptConnection, ServerMessage message)
  {
    foreach (var index in connections.GetFilledSlots())
    {
      var connection = connections.Get(index);
      if (connection?.WebSocket != null && !Equals(connection, exceptConnection))
      {
        try
        {
          DataTo(connection, message);
        }
        catch (Exception ex)
        {
          Logger.Error("Error sending data to the client! Error: " + ex.Message);
        }
      }
    }
  }
}
