using Throne.Server.Websocket;

namespace Throne.Server.Configurations;

public static class WebSocketConfiguration
{
    public static void UseWebSocketHandling(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
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
                await next();
            }
        });
    }
}