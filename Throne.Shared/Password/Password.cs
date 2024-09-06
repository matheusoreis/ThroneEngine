using BC = BCrypt.Net.BCrypt;

namespace Throne.Shared.Password;

public class Password
{
  public static string Hash(string password)
  {
    return BC.HashPassword(password);
  }

  public static bool Verify(string password, string hashedPassword)
  {
    return BC.Verify(password, hashedPassword);
  }
}
