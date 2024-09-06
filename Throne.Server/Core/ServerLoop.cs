using System.Net;
using Throne.Server.Network;

namespace Throne.Server.Core;

public class ServerLoop
{
  private const int loopUpdateInterval = 100;
  private const int npcUpdateInterval = 100;
  private static DateTime lastNpcUpdate = DateTime.Now;

  public static async Task Start()
  {
    while (true)
    {
      var now = DateTime.Now;

      if ((now - lastNpcUpdate).TotalMilliseconds >= npcUpdateInterval)
      {
        await UpdateNpcs();
        lastNpcUpdate = now;
      }

      await Task.Delay(loopUpdateInterval);
    }
  }

  private static Task UpdateNpcs()
  {
    return Task.CompletedTask;
  }
}
