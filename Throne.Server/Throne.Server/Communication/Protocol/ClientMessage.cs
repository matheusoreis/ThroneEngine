using Throne.Shared.Communication;

namespace Throne.Server.Communication.Protocol;

public class ClientMessage(byte[] buffer)
{
  private readonly ByteBuffer buffer = new(buffer);

  public short GetId()
  {
    return buffer.GetInt16();
  }

  public byte[] GetContent()
  {
    return buffer.GetBytes(buffer.GetBuffer().Length - buffer.GetOffset());
  }

  public sbyte GetInt8()
  {
    return buffer.GetInt8();
  }

  public short GetInt16()
  {
    return buffer.GetInt16();
  }

  public int GetInt32()
  {
    return buffer.GetInt32();

  }

  public string GetString()
  {
    return buffer.GetString();
  }

  public byte[] GetBytes(int length)
  {
    return buffer.GetBytes(length);
  }
}
