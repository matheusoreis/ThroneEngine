using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Throne.Server.Communication.Protocol;
using Throne.Server.Core;
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
    MemoryManager? memoryManager = (ServiceLocator.ServiceProvider?.GetRequiredService<MemoryManager>()) ?? throw new InvalidOperationException("MemoryManager is not registered in the ServiceLocator.");
    connections = memoryManager.Connections;
  }

  protected static async Task DataTo(WebSocketConnection connection, ServerMessage serverMessage)
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

  protected async Task DataToAllExcept(WebSocketConnection exceptConnection, ServerMessage message)
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
