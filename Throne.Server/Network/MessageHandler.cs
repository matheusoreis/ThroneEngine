using Throne.Server.Communication.Incoming;
using Throne.Server.Communication.Protocol;
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

  public void ProcessMessage(WebSocketConnection connection, ClientMessage message)
  {
    if (connection == null) return;
    if (message == null) return;

    short messageId = message.GetId();

    if (Enum.IsDefined(typeof(ClientHeaders), messageId))
    {
      ClientHeaders header = (ClientHeaders)messageId;

      if (messageHandlers.TryGetValue(header, out var handler))
      {
        try
        {
          handler.Handle(connection, message);
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

  private static void InitializeHandlers() { }
}
