using System.Net;
using System.Net.WebSockets;
using Throne.Server.Network;
using Throne.Shared.Constants;
using Throne.Shared.Logger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Throne.Server.Core.Memory;
using Throne.Server.Core;

namespace Throne.Server;

class Program
{
  static async Task Main()
  {
    Logger.Info($"Iniciando o servidor Throne...");

    IHost? host = Host.CreateDefaultBuilder()
    .ConfigureServices(ConfigureServices)
    .Build();

    ServiceLocator.ServiceProvider = host.Services;

    Logger.Info($"Configurando o gerenciador de WebSocket...");
    WebsocketManager websocketManager = ServiceLocator.ServiceProvider.GetRequiredService<WebsocketManager>();
    HttpListener? httpListener = new();

    Logger.Info($"Iniciando o HttpListener...");
    httpListener.Prefixes.Add($"http://{Constants.ServerHost}:{Constants.Port}/");
    httpListener.Start();

    Logger.Info($"Servidor WebSocket iniciado com sucesso!");
    Logger.Info($"Endereço de conexão WebSocket: ws://{Constants.ServerHost}/");
    Logger.Info($"Porta: {Constants.Port}");

    Logger.Info("Aguardando por novas conexões.");

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
        Logger.Info($"Nova conexão WebSocket recebida de {ip}");

        _ = websocketManager.HandleWebSocketConnection(wsContext, ip);
      }
      catch (Exception e)
      {
        Logger.Error($"Erro ao aceitar conexão WebSocket: {e.Message}");
      }
    }
  }

  static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
  {
    Logger.Info("Registrando dependências...");
    services.AddSingleton<MemoryManager>();
    services.AddSingleton<WebsocketManager>();
    Logger.Info("Dependências registradas com sucesso.");
  }
}
