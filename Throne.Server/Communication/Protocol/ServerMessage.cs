using Throne.Server.Communication.Outgoing;
using Throne.Server.Network;
using Throne.Shared.Communication;

namespace Throne.Server.Communication.Protocol;

public class ServerMessage : OutgoingMessage
{
  private readonly ByteBuffer buffer;

  public ServerMessage(short id)
  {
    buffer = new ByteBuffer();
    buffer.PutInt16(id);
  }

  protected void PutBytes(byte[] buffer)
  {
    this.buffer.PutBytes(buffer);
  }

  protected void PutInt8(sbyte value)
  {
    buffer.PutInt8(value);
  }

  protected void PutInt16(short value)
  {
    buffer.PutInt16(value);
  }

  protected void PutInt32(int value)
  {
    buffer.PutInt32(value);
  }

  protected void PutString(string value)
  {
    buffer.PutString(value);
  }

  public byte[] GetBuffer()
  {
    return buffer.GetBuffer();
  }

  public async Task SendTo(WebSocketConnection connection)
  {
    await DataTo(connection, this);
  }

  public async Task SendToAll()
  {
    await DataToAll(this);
  }

  public async Task SendToAllExcept(WebSocketConnection exceptConnection)
  {
    await DataToAllExcept(exceptConnection, this);
  }

}
