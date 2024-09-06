using System.Net;
using System.Net.WebSockets;
using Throne.Server.Network;
using Throne.Shared.Constants;
using Throne.Shared.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Throne.Shared.Slots;
using Throne.Server.Core.Memory;

namespace Throne.Server;


class Program
{
  static async Task Main()
  {
    var host = Host.CreateDefaultBuilder()
    .ConfigureServices(ConfigureServices)
    .Build();

    var websocketManager = host.Services.GetRequiredService<WebsocketManager>();

    var httpListener = new HttpListener();

    httpListener.Prefixes.Add($"http://{Constants.ServerHost}:{Constants.Port}/");
    httpListener.Start();

    Logger.Info($"Servidor WebSocket iniciado em ws://{Constants.ServerHost}:{Constants.Port}/");
    Logger.Info("Aguardando conexões...");


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

  static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
  {
    services.AddSingleton(new Slots<WebSocketConnection>(Constants.MaxConnections));
    services.AddSingleton<MemoryManager>();
    services.AddSingleton<WebsocketManager>();
  }
}
