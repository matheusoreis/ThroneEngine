using Throne.Server.Services;
using Throne.Server.Utils;

namespace Throne.Server.Websocket.Core.Memory;

public class MemoryManager : IMemoryManager
{
    public Slots<WsConnection> Connections { get; } = new(Constants.MaxConnections);
}