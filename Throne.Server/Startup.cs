using System.Net.WebSockets;
using Throne.Server.Websocket;
using Throne.Server.Websocket.Core;
using Throne.Server.Websocket.Core.Memory;

namespace Throne.Server;

public class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    services.AddControllers();
    services.AddSingleton<IMemoryManager, MemoryManager>();
    services.AddSingleton<WSManager>();
  }

  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseWebSockets();
    app.UseAuthorization();

    app.Use(async (context, next) =>
    {
      await HandleWebSocketRequest(context);
      await next();
    });

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapControllers();
    });

    var serviceProvider = app.ApplicationServices;
    ServiceLocator.SetServiceProvider(serviceProvider);
  }

  private static async Task HandleWebSocketRequest(HttpContext context)
  {
    if (context.Request.Path == "/ws")
    {
      if (context.WebSockets.IsWebSocketRequest)
      {
        WebSocket? webSocket = await context.WebSockets.AcceptWebSocketAsync();
        string? ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var websocketManager = context.RequestServices.GetRequiredService<WSManager>();
        await websocketManager.HandleWebSocketConnection(webSocket, ip);
      }
      else
      {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
      }
    }
    else
    {
      await Task.CompletedTask;
    }
  }
}
