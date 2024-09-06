using System.Net;
using System.Net.WebSockets;
using Throne.Server.Network;
using Throne.Shared.Constants;
using Throne.Shared.Logger;

namespace Throne.Server;


class Program
{
  static async Task Main()
  {
    HttpListener httpListener = new();
    httpListener.Prefixes.Add($"http://{Constants.ServerHost}:{Constants.Port}/");
    httpListener.Start();
    Logger.Info($"Servidor WebSocket iniciado em ws://{Constants.ServerHost}:{Constants.Port}/");
    Logger.Info("Aguardando conexões...");

    WebsocketManager websocketManager = new();

    while (true)
    {
      try
      {
        HttpListenerContext context = await httpListener.GetContextAsync();

        if (!context.Request.IsWebSocketRequest)
        {
          context.Response.StatusCode = 400;
          context.Response.Close();
          continue;
        }

        string ip = context.Request.RemoteEndPoint?.Address.ToString() ?? "Unknown";
        HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);

        _ = websocketManager.HandleWebSocketConnection(wsContext, ip);
      }
      catch (Exception e)
      {
        Logger.Error($"Erro ao aceitar conexão: {e.Message}");
      }
    }
  }
}
