using Throne.Shared.Constants;
using Throne.Shared.Slots;

namespace Throne.Server.Websocket.Core.Memory;

public class MemoryManager : IMemoryManager
{
    public Slots<WSConnection> Connections { get; } = new(Constants.MaxConnections);
}