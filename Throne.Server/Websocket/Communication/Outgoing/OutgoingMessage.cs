using System.Net.WebSockets;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Server.Websocket.Core;
using Throne.Server.Websocket.Core.Memory;
using Throne.Shared.Logger;
using Throne.Shared.Slots;

namespace Throne.Server.Websocket.Communication.Outgoing;

public abstract class OutgoingMessage
{
  private readonly Slots<WSConnection> connections;

  protected OutgoingMessage()
  {
    var serviceProvider = ServiceLocator.GetServiceProvider();
    var memoryManager = serviceProvider.GetRequiredService<IMemoryManager>();
    connections = memoryManager.Connections;
  }

  protected static async Task DataTo(WSConnection connection, ServerMessage serverMessage)
  {
    try
    {
      WebSocketMessageType webSocketType = WebSocketMessageType.Binary;
      CancellationToken tokenType = CancellationToken.None;
      await connection.WebSocket.SendAsync(serverMessage.GetBuffer(), webSocketType, true, tokenType);
    }
    catch (Exception e)
    {
      Logger.Error($"Error sending data to the client! Error: {e.Message}");
    }
  }

  protected async Task DataToAll(ServerMessage message)
  {
    foreach (var index in connections.GetFilledSlots())
    {
      var connection = connections.Get(index);

      if (connection?.WebSocket != null)
      {
        try
        {
          await DataTo(connection, message);
        }
        catch (Exception e)
        {
          Logger.Error("Error sending data to the client! Error: " + e.Message);
        }
      }
    }
  }

  protected async Task DataToAllExcept(WSConnection exceptConnection, ServerMessage message)
  {
    foreach (var index in connections.GetFilledSlots())
    {
      var connection = connections.Get(index);
      if (connection?.WebSocket != null && !Equals(connection, exceptConnection))
      {
        try
        {
          await DataTo(connection, message);
        }
        catch (Exception e)
        {
          Logger.Error("Error sending data to the client! Error: " + e.Message);
        }
      }
    }
  }
}
