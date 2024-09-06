using Throne.Server.Network;
using Throne.Shared.Constants;
using Throne.Shared.Slots;

namespace Throne.Server.Core.Memory
{
  public class MemoryManager()
  {
    public Slots<WebSocketConnection> Connections { get; } = new Slots<WebSocketConnection>(Constants.MaxConnections);
  }
}
