
using System.Buffers;
using Throne.Server.Communication.Incoming;
using Throne.Server.Communication.Incoming.Requests;
using Throne.Server.Communication.Protocol;
using Throne.Server.Core.Memory;
using Throne.Shared.Communication;
using Throne.Shared.Logger;

namespace Throne.Server.Network;

public class MessageHandler
{
  private readonly Dictionary<ClientHeaders, IIncoming> messageHandlers = [];

  public MessageHandler()
  {
    InitializeHandlers();
  }

  public async Task ProcessMessage(WebSocketConnection connection, ClientMessage message)
  {
    if (connection == null || message == null)
    {
      return;
    }

    int messageId = message.GetId();

    if (Enum.IsDefined(typeof(ClientHeaders), messageId))
    {
      ClientHeaders header = (ClientHeaders)messageId;

      if (messageHandlers.TryGetValue(header, out var handler))
      {
        try
        {
          await handler.Handle(connection, message);
        }
        catch (Exception e)
        {
          Logger.Error($"Error handling {handler.GetType().Name}: {e}");
          _ = connection.Close();
        }
      }
      else
      {
        Logger.Error($"No handler for id: {header}");
        _ = connection.Close();
      }
    }
    else
    {
      Logger.Error($"Invalid message ID: {messageId}");
      _ = connection.Close();
    }
  }

  private void InitializeHandlers()
  {
    messageHandlers[ClientHeaders.Ping] = new PingRequest();
  }
}
