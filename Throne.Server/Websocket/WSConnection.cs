using System.Net.WebSockets;
using Throne.Server.Utils;
using Throne.Server.Websocket.Communication.Protocol;

namespace Throne.Server.Websocket;

public class WsConnection(WebSocket webSocket, int id, string ip)
{
    public WebSocket WebSocket { get; } = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
    public int Id { get; } = id;
    public string Ip { get; } = ip;
    private WsHandler Handler { get; } = new();

    public async Task Close()
    {
        if (WebSocket.State == WebSocketState.Open || WebSocket.State == WebSocketState.CloseSent)
            try
            {
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection",
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.Error($"Erro ao fechar a conexão com o IP {Ip}, ID da conexão: {Id}. Erro: {ex}");
            }
        else
            Logger.Warning($"Erro ao fechar conexão, o WebSocket não está aberto. Estado atual: {WebSocket.State}");
    }

    public bool IsOpen()
    {
        return WebSocket.State == WebSocketState.Open;
    }

    public async Task ProcessMessage(byte[] buffer)
    {
        ClientMessage message = new(buffer);
        await Handler.ProcessMessage(this, message);
    }
}