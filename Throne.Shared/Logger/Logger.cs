namespace Throne.Shared.Logger;

public sealed class Logger
{
  private const int GREEN = 32;
  private const int YELLOW = 33;
  private const int BLUE = 34;
  private const int RED = 31;

  private static string GetTimestamp()
  {
    return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
  }

  private static string ColorText(string text, int colorCode)
  {
    return $"\x1b[{colorCode}m{text}\x1b[0m";
  }

  public static void Info(string message)
  {
    Console.WriteLine($"{ColorText("[INFO]", GREEN)} {GetTimestamp()} - {ColorText(message, GREEN)}");
  }

  public static void Warning(string message)
  {
    Console.WriteLine($"{ColorText("[WARN]", YELLOW)} {GetTimestamp()} - {ColorText(message, YELLOW)}");
  }

  public static void Player(string message)
  {
    Console.WriteLine($"{ColorText("[PLAYER]", BLUE)} {GetTimestamp()} - {ColorText(message, BLUE)}");
  }

  public static void Error(string message)
  {
    Console.WriteLine($"{ColorText("[ERROR]", RED)} {GetTimestamp()} - {ColorText(message, RED)}");
  }
}

