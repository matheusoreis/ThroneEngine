using Throne.Shared.Slots;

namespace Throne.Server.Websocket.Core.Memory;

public interface IMemoryManager
{
    Slots<WSConnection> Connections { get; }
}