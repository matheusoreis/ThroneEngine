using Throne.Server.Network;
using Throne.Shared.Slots;

namespace Throne.Server.Core.Memory
{
  public class MemoryManager(Slots<WebSocketConnection> connections)
  {
    public Slots<WebSocketConnection> Connections { get; } = connections;
  }
}
