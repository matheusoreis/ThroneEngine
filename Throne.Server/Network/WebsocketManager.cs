using System.Net.WebSockets;
using Throne.Server.Communication.Outgoing.Messages;
using Throne.Server.Core.Memory;
using Throne.Shared.Logger;
using Throne.Shared.Slots;

namespace Throne.Server.Network;

public class WebsocketManager(MemoryManager memoryManager)
{
  private readonly Slots<WebSocketConnection> connections = memoryManager.Connections;

  public async Task HandleWebSocketConnection(HttpListenerWebSocketContext wsContext, string ip)
  {
    WebSocket webSocket = wsContext.WebSocket;
    List<byte>? receivedData = [];
    var cancellationTokenSource = new CancellationTokenSource();
    var cancellationToken = cancellationTokenSource.Token;

    try
    {
      Logger.Info($"Nova conexão WebSocket aberta com IP: {ip}");
      await WebSocketOpen(webSocket, ip);

      byte[]? buffer = new byte[1024 * 4];
      while (webSocket.State == WebSocketState.Open)
      {
        ArraySegment<byte> segment = new(buffer);
        try
        {
          WebSocketReceiveResult result = await webSocket.ReceiveAsync(segment, cancellationToken);
          receivedData.AddRange(buffer.Take(result.Count));

          if (result.MessageType == WebSocketMessageType.Close)
          {
            Logger.Info($"Conexão WebSocket com {ip} solicitou fechamento.");
            await WebSocketClose(webSocket);
            break;
          }

          if (result.EndOfMessage)
          {
            await WebSocketMessage(webSocket, receivedData.ToArray());
            receivedData.Clear();
          }
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
          Logger.Warning($"Conexão WebSocket com {ip} foi fechada abruptamente.");
          break;
        }
        catch (WebSocketException ex)
        {
          Logger.Error($"WebSocketException durante a recepção de dados de {ip}: {ex.Message}");
          break;
        }
        catch (Exception ex)
        {
          Logger.Error($"Erro inesperado durante a recepção de dados de {ip}: {ex.Message}");
          break;
        }
      }
    }
    catch (Exception ex)
    {
      Logger.Error($"Erro inesperado ao lidar com a conexão WebSocket com {ip}: {ex.Message}");
    }
    finally
    {
      await CleanupConnection(webSocket);
    }
  }

  public async Task WebSocketOpen(WebSocket webSocket, string ip)
  {
    int? connectionId = connections.GetFirstEmptySlot();

    if (connectionId == null)
    {
      Logger.Warning($"Servidor cheio, não há slots disponíveis para {ip}");
      await HandleFullServer(webSocket, ip);
      return;
    }

    WebSocketConnection? connection = new(webSocket, connectionId.Value, ip);
    connections.Add(connection);
    Logger.Info($"Conexão estabelecida com {ip}, ID da conexão: {connectionId.Value}");
  }

  public async Task WebSocketMessage(WebSocket webSocket, byte[] message)
  {
    var connection = GetConnectionBySocket(webSocket);

    if (connection == null) return;

    try
    {
      if (!connection.IsOpen()) return;

      await connection.ProcessMessage(message);
    }
    catch (Exception ex)
    {
      Logger.Error($"Erro ao processar a mensagem para o IP {connection.Ip}: {ex.Message}");
      await CleanupConnection(webSocket);
    }
  }


  public async Task WebSocketClose(WebSocket webSocket)
  {
    try
    {
      var connection = GetConnectionBySocket(webSocket);

      if (connection != null)
      {
        Logger.Info($"Fechando conexão com IP: {connection.Ip}, ID da conexão: {connection.Id}");
        await CleanupConnection(webSocket);
      }
    }
    catch (WebSocketException ex)
    {
      Logger.Error($"WebSocketException ao fechar a conexão: {ex.Message}");
    }
    catch (Exception ex)
    {
      Logger.Error($"Erro inesperado ao fechar a conexão: {ex.Message}");
    }
  }



  private async Task HandleFullServer(WebSocket webSocket, string ip)
  {
    Logger.Info($"Servidor está cheio, desconectando o IP: {ip}");

    WebSocketConnection webSocketConnection = new(webSocket, -1, ip);

    AlertData alertData = new()
    {
      Type = AlertType.Info,
      Message = "O servidor está cheio, desconectando..."
    };

    AlertMessage alertMessage = new(alertData);

    await alertMessage.SendTo(webSocketConnection);
  }


  private async Task CleanupConnection(WebSocket webSocket)
  {
    var connection = GetConnectionBySocket(webSocket);

    if (connection != null)
    {
      connections.Remove(connection.Id);
      Logger.Info($"Conexão removida com IP {connection.Ip}, ID da conexão: {connection.Id}");

      if (connection.IsOpen())
      {
        try
        {
          await connection.Close();
        }
        catch (WebSocketException ex)
        {
          Logger.Error($"WebSocketException ao fechar a conexão com IP {connection.Ip}, ID da conexão {connection.Id}: {ex.Message}");
        }
        catch (Exception ex)
        {
          Logger.Error($"Erro ao fechar a conexão com IP {connection.Ip}, ID da conexão {connection.Id}: {ex.Message}");
        }
      }
    }
  }

  private WebSocketConnection? GetConnectionBySocket(WebSocket webSocket)
  {
    foreach (var index in connections.GetFilledSlots())
    {
      var connection = connections.Get(index);

      if (connection?.WebSocket == webSocket)
      {
        return connection;
      }
    }
    return null;
  }
}
