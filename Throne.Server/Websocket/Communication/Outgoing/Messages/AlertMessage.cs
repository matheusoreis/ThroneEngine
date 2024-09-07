using Throne.Server.Websocket.Communication.Protocol;
using Throne.Shared.Communication;

namespace Throne.Server.Websocket.Communication.Outgoing.Messages;

public enum AlertType
{
  Info,
  Warn,
  Error
}

public record AlertData
{
  public AlertType Type { get; set; }
  public required string Message { get; set; }
}

public class AlertMessage : ServerMessage
{
  public AlertMessage(AlertData data) : base((short)ServerHeaders.Alert)
  {
    PutInt8((sbyte)data.Type);
    PutString(data.Message);
  }
}
