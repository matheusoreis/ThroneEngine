using Throne.Server.Utils;
using Throne.Server.Websocket.Communication;
using Throne.Server.Websocket.Communication.Incoming;
using Throne.Server.Websocket.Communication.Incoming.Requests;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Shared.Communication;

namespace Throne.Server.Websocket;

public class WsHandler
{
    private readonly Dictionary<ClientHeaders, IIncoming> messageHandlers = [];

    public WsHandler()
    {
        InitializeHandlers();
    }

    public async Task ProcessMessage(WsConnection connection, ClientMessage message)
    {
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
        messageHandlers[ClientHeaders.SignUp] = new SignUpRequest();
        messageHandlers[ClientHeaders.CharList] = new CharListRequest();
        messageHandlers[ClientHeaders.DeleteChar] = new DeleteCharRequest();
    }
}