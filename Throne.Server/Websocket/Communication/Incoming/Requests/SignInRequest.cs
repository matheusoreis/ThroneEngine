using Npgsql;
using Throne.Server.Websocket;
using Throne.Server.Websocket.Communication.Incoming;
using Throne.Server.Websocket.Communication.Outgoing.Messages;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Shared.Constants;
using Throne.Shared.Database;
using Throne.Shared.Logger;
using Throne.Shared.Password;
using Throne.Shared.VersionsChecker;

namespace Throne.Server.Websocket.Communication.Incoming.Requests;

public class SignInRequest : IIncoming
{
  public async Task Handle(WSConnection connection, ClientMessage clientMessage)
  {
    string email = clientMessage.GetString();
    string password = clientMessage.GetString();
    int major = clientMessage.GetInt16();
    int minor = clientMessage.GetInt16();
    int revision = clientMessage.GetInt16();

    VersionChecker versionChecker = new(Constants.MajorVersion, Constants.MinorVersion, Constants.PatchVersion);

    if (!versionChecker.Check(major, minor, revision))
    {
      AlertData alertData = new()
      {
        Type = AlertType.Info,
        Message = "Ops! a versão do seu cliente está desatualizada!"
      };

      AlertMessage alertMessage = new(alertData);

      await alertMessage.SendTo(connection);

      return;
    }

    var database = new Database();

    try
    {
      var emailExistsQuery = "SELECT check_email_exists(@p_email)";
      var emailExistsParameters = new[] { new NpgsqlParameter("@p_email", email) };

      bool emailExists = await database.ExecutarFunctionAsync<bool>(emailExistsQuery, emailExistsParameters);

      if (emailExists)
      {
        var getHashedPasswordQuery = "SELECT password FROM accounts WHERE email = @p_email";
        var getHashedPasswordParameters = new[] { new NpgsqlParameter("@p_email", email) };
        string? hashedPassword = await database.ExecutarFunctionAsync<string>(getHashedPasswordQuery, getHashedPasswordParameters);

        if (hashedPassword != null && Password.Verify(password, hashedPassword))
        {
          AlertData alertData = new()
          {
            Type = AlertType.Info,
            Message = "Login bem-sucedido!"
          };

          AlertMessage alertMessage = new(alertData);

          await alertMessage.SendTo(connection);
        }
        else
        {
          AlertData alertData = new()
          {
            Type = AlertType.Error,
            Message = "Senha incorreta!"
          };

          AlertMessage alertMessage = new(alertData);

          await alertMessage.SendTo(connection);
        }
      }
      else
      {
        AlertData alertData = new()
        {
          Type = AlertType.Warn,
          Message = "E-mail não encontrado!"
        };

        AlertMessage alertMessage = new(alertData);

        await alertMessage.SendTo(connection);
      }
    }
    catch (Exception ex)
    {
      Logger.Error($"Erro ao buscar accounts: {ex.Message}");
    }
    finally
    {
      await database.CloseConnectionAsync();
    }
  }
}
