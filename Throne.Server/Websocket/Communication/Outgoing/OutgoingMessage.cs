using System.Net.WebSockets;
using Throne.Server.Services;
using Throne.Server.Utils;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Server.Websocket.Core;
using Throne.Server.Websocket.Core.Memory;

namespace Throne.Server.Websocket.Communication.Outgoing;

public abstract class OutgoingMessage
{
    private readonly Slots<WsConnection> connections;

    protected OutgoingMessage()
    {
        IServiceProvider serviceProvider = ServiceLocator.GetServiceProvider();
        IMemoryManager memoryManager = serviceProvider.GetRequiredService<IMemoryManager>();
        connections = memoryManager.Connections;
    }

    protected static async Task DataTo(WsConnection connection, ServerMessage serverMessage)
    {
        try
        {
            WebSocketMessageType webSocketType = WebSocketMessageType.Binary;
            CancellationToken tokenType = CancellationToken.None;
            await connection.WebSocket.SendAsync(serverMessage.GetBuffer(), webSocketType, true, tokenType);
        }
        catch (Exception e)
        {
            Logger.Error($"Error sending data to the client! Error: {e.Message}");
        }
    }

    protected async Task DataToAll(ServerMessage message)
    {
        foreach (int index in connections.GetFilledSlots())
        {
            WsConnection? connection = connections.Get(index);

            if (connection?.WebSocket != null)
                try
                {
                    await DataTo(connection, message);
                }
                catch (Exception e)
                {
                    Logger.Error("Error sending data to the client! Error: " + e.Message);
                }
        }
    }

    protected async Task DataToAllExcept(WsConnection exceptConnection, ServerMessage message)
    {
        foreach (int index in connections.GetFilledSlots())
        {
            WsConnection? connection = connections.Get(index);
            if (connection?.WebSocket != null && !Equals(connection, exceptConnection))
                try
                {
                    await DataTo(connection, message);
                }
                catch (Exception e)
                {
                    Logger.Error("Error sending data to the client! Error: " + e.Message);
                }
        }
    }
}