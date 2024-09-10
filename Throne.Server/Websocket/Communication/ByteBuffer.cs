using System.Text;

namespace Throne.Shared.Communication;

public class ByteBuffer
{
  private byte[] buffer;
  private int offset;

  public ByteBuffer() : this(new byte[0]) { }

  public ByteBuffer(byte[] initialBuffer)
  {
    buffer = initialBuffer;
    offset = 0;
  }

  public void PutBytes(byte[] bytes)
  {
    var newBuffer = new byte[buffer.Length + bytes.Length];
    Array.Copy(buffer, newBuffer, buffer.Length);
    Array.Copy(bytes, 0, newBuffer, buffer.Length, bytes.Length);
    buffer = newBuffer;
  }

  public void PutInt8(sbyte value)
  {
    PutBytes([(byte)value]);
  }

  public void PutInt16(short value)
  {
    var bytes = BitConverter.GetBytes(value);
    PutBytes(bytes);
  }

  public void PutInt32(int value)
  {
    var bytes = BitConverter.GetBytes(value);
    PutBytes(bytes);
  }

  public void PutString(string value)
  {
    var stringBytes = Encoding.UTF8.GetBytes(value);
    PutInt32(stringBytes.Length);
    PutBytes(stringBytes);
  }

  public sbyte GetInt8()
  {
    var value = (sbyte)buffer[offset];
    offset += 1;
    return value;
  }

  public short GetInt16()
  {
    var value = BitConverter.ToInt16(buffer, offset);
    offset += 2;
    return value;
  }

  public int GetInt32()
  {
    var value = BitConverter.ToInt32(buffer, offset);
    offset += 4;
    return value;
  }

  public string GetString()
  {
    var length = GetInt32();
    var value = Encoding.UTF8.GetString(buffer, offset, length);
    offset += length;
    return value;
  }

  public byte[] GetBytes(int length)
  {
    var bytes = new byte[length];
    Array.Copy(buffer, offset, bytes, 0, length);
    offset += length;
    return bytes;
  }

  public byte[] GetBuffer()
  {
    return buffer;
  }

  public int GetOffset()
  {
    return offset;
  }

  public void SetOffset(int offset)
  {
    this.offset = offset;
  }
}
