namespace Throne.Server.Utils;

public static class Logger
{
  private const int Green = 32;
  private const int Yellow = 33;
  private const int Blue = 34;
  private const int Red = 31;

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
    Console.WriteLine($"{ColorText("[INFO]", Green)} {GetTimestamp()} - {ColorText(message, Green)}");
  }

  public static void Warning(string message)
  {
    Console.WriteLine($"{ColorText("[WARN]", Yellow)} {GetTimestamp()} - {ColorText(message, Yellow)}");
  }

  public static void Player(string message)
  {
    Console.WriteLine($"{ColorText("[PLAYER]", Blue)} {GetTimestamp()} - {ColorText(message, Blue)}");
  }

  public static void Error(string message)
  {
    Console.WriteLine($"{ColorText("[ERROR]", Red)} {GetTimestamp()} - {ColorText(message, Red)}");
  }
}

