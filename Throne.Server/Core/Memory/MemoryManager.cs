using Throne.Server.Network;
using Throne.Shared.Constants;
using Throne.Shared.Slots;

namespace Throne.Server.Core.Memory;

public class MemoryManager
{
  private static readonly Lazy<MemoryManager> instance = new(() => new MemoryManager());
  public static MemoryManager Instance => instance.Value;

  public Slots<WebSocketConnection> Connections { get; }

  private MemoryManager()
  {
    Connections = new Slots<WebSocketConnection>(Constants.MaxConnections);
  }
}
