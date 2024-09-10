using Throne.Server.Services;
using Throne.Server.Utils;

namespace Throne.Server.Websocket.Core.Memory;

public interface IMemoryManager
{
    Slots<WsConnection> Connections { get; }
}