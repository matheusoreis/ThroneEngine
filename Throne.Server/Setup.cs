using System.Net;
using System.Net.WebSockets;
using Throne.Server.Network;
using Throne.Shared.Constants;
using Throne.Shared.Logger;

namespace Throne.Server;

public class Setup
{
  internal class Program
  {
    static async Task Main()
    {
      HttpListener httpListener = new();
      httpListener.Prefixes.Add($"http://{Constants.ServerHost}:{Constants.Port}/");
      httpListener.Start();
      Logger.Info($"Servidor WebSocket iniciado em ws://{Constants.ServerHost}:{Constants.Port}/");
      Logger.Info("Aguardando conexões...");

      WebsocketManager? websocketManager = new();

      while (true)
      {
        try
        {
          HttpListenerContext context = await httpListener.GetContextAsync();

          if (context.Request.IsWebSocketRequest)
          {
            HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
            WebSocket webSocket = wsContext.WebSocket;

            string ip = context.Request.RemoteEndPoint?.Address.ToString() ?? "Unknown";
            _ = HandleWebSocketCommunication(webSocket, websocketManager, ip);
          }
          else
          {
            context.Response.StatusCode = 400;
            context.Response.Close();
          }
        }
        catch (Exception e)
        {
          Console.WriteLine($"Erro ao aceitar conexão: {e.Message}");
        }
      }
    }

    private static async Task HandleWebSocketCommunication(WebSocket webSocket, WebsocketManager websocketManager, string clientIp)
    {
      var receivedData = new List<byte>();

      try
      {
        await websocketManager.WebSocketOpen(webSocket, clientIp);

        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
          var segment = new ArraySegment<byte>(buffer);
          WebSocketReceiveResult result = await webSocket.ReceiveAsync(segment, CancellationToken.None);

          receivedData.AddRange(buffer.Take(result.Count));

          if (result.EndOfMessage)
          {
            byte[] data = [.. receivedData];
            receivedData.Clear();

            await websocketManager.WebSocketMessage(webSocket, data);
          }

          if (result.MessageType == WebSocketMessageType.Close)
          {
            await websocketManager.WebSocketClose(webSocket);
            break;
          }
        }
      }
      catch (Exception)
      {
        await websocketManager.WebSocketClose(webSocket);
      }
    }

  }
}
