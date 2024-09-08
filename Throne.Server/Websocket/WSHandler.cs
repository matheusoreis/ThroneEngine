using Throne.Server.Websocket.Communication.Incoming;
using Throne.Server.Websocket.Communication.Incoming.Requests;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Shared.Communication;
using Throne.Shared.Logger;

namespace Throne.Server.Websocket;

public class WSHandler
{
    private readonly Dictionary<ClientHeaders, IIncoming> messageHandlers = [];

    public WSHandler()
    {
        InitializeHandlers();
    }

    public async Task ProcessMessage(WSConnection connection, ClientMessage message)
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

            if (messageHandlers.TryGetValue(header, out IIncoming? handler))
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
        messageHandlers[ClientHeaders.SignIn] = new SignInRequest();
    }
}