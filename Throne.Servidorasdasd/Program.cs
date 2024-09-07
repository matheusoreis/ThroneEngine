using System.Net.WebSockets;
using Throne.Server.Network;
using Throne.Server.Core.Memory;
using Throne.Servidor.Core.Memory;
using Throne.Server.Core;
using Microsoft.OpenApi.Models;

namespace Throne.Server;
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.Configure(ConfigureApp));
    }

    private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.AddSingleton<IMemoryManager, MemoryManager>();
        services.AddSingleton<WebsocketManager>();
        services.AddAuthorization();
        services.AddControllers();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Throne Server API", Version = "v1" });
        });

        var serviceProvider = services.BuildServiceProvider();
        ServiceLocator.SetServiceProvider(serviceProvider);
    }

    private static void ConfigureApp(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Throne Server API V1");
            c.RoutePrefix = "swagger";
        });

        app.UseWebSockets();
        app.Use(async (context, next) =>
        {
            await HandleWebSocketRequest(context);
            await next();
        });
    }

    private static async Task HandleWebSocketRequest(HttpContext context)
    {
        if (context.Request.Path == "/ws")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket? webSocket = await context.WebSockets.AcceptWebSocketAsync();
                string? ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

                var websocketManager = context.RequestServices.GetRequiredService<WebsocketManager>();

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
