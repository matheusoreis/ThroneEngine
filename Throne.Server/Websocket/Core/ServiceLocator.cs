namespace Throne.Server.Websocket.Core;

public static class ServiceLocator
{
  private static IServiceProvider? _serviceProvider;

  public static void SetServiceProvider(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public static IServiceProvider GetServiceProvider()
  {
    if (_serviceProvider == null)
    {
      throw new InvalidOperationException("ServiceProvider has not been set.");
    }
    return _serviceProvider;
  }
}