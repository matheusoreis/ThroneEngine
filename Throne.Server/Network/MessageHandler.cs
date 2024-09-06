
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
      Logger.Warning("Conexão ou mensagem nula recebida ao tentar processar a mensagem.");
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
          Logger.Error($"Erro ao processar a mensagem com header {header}: {e}");
          _ = connection.Close();
        }
      }
      else
      {
        Logger.Warning($"Nenhum handler encontrado para o header: {header}.");
        _ = connection.Close();
      }
    }
    else
    {
      Logger.Error($"ID de mensagem inválido recebido: {messageId}.");
      _ = connection.Close();
    }
  }

  private void InitializeHandlers()
  {
    messageHandlers[ClientHeaders.Ping] = new PingRequest();
  }
}
