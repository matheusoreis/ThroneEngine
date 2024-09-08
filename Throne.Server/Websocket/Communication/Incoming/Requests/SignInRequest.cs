using Npgsql;
using Throne.Server.Websocket.Communication.Outgoing.Messages;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Server.Websocket.Core;
using Throne.Shared.Database;
using Throne.Shared.Logger;
using Throne.Shared.VersionsChecker;
using BC = BCrypt.Net.BCrypt;

namespace Throne.Server.Websocket.Communication.Incoming.Requests;

internal class AccountInfo
{
    public int Id { get; init; }
    public required string Password { get; init; }
}

public class SignInRequest : IIncoming
{
    private readonly IDatabase database;
    private readonly VersionChecker versionChecker;

    public SignInRequest()
    {
        IServiceProvider serviceProvider = ServiceLocator.GetServiceProvider();
        database = serviceProvider.GetRequiredService<IDatabase>();
        versionChecker = serviceProvider.GetRequiredService<VersionChecker>();
    }

    public async Task Handle(WSConnection connection, ClientMessage clientMessage)
    {
        string email = clientMessage.GetString();
        string password = clientMessage.GetString();
        int major = clientMessage.GetInt16();
        int minor = clientMessage.GetInt16();
        int revision = clientMessage.GetInt16();

        if (!versionChecker.Check(major, minor, revision))
        {
            await SendAlert(connection, AlertType.Info, "Ops! a versão do seu cliente está desatualizada!");
            return;
        }

        try
        {
            const string checkEmailQuery = "SELECT check_email_exists(@p_email)";
            NpgsqlParameter[] emailExistsParameters =
            [
                new NpgsqlParameter("@p_email", email)
            ];

            bool emailExists = await database.ExecuteFunction<bool>(checkEmailQuery, emailExistsParameters);
            if (emailExists)
            {
                const string getPasswordQuery = "SELECT id, password FROM accounts WHERE email = @p_email";
                NpgsqlParameter[] passwordParameters =
                [
                    new NpgsqlParameter("@p_email", email)
                ];

                List<AccountInfo> accountInfos = await database.RunQuery(getPasswordQuery, reader => new AccountInfo
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Password = reader.GetString(reader.GetOrdinal("password"))
                }, passwordParameters);

                AccountInfo? accountInfo = accountInfos.FirstOrDefault();

                if (accountInfo != null && BC.Verify(password, accountInfo.Password))
                {
                    await SendAlert(connection, AlertType.Info, "Login bem-sucedido!");
                    SignInMessage signInMessage = new SignInMessage(accountInfo.Id);
                    await signInMessage.SendTo(connection);
                }
                else
                {
                    await SendAlert(connection, AlertType.Warn, "Senha incorreta!");
                }
            }
            else
            {
                await SendAlert(connection, AlertType.Warn, "E-mail não encontrado!");
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Erro ao buscar accounts: {ex.Message}");
        }
        finally
        {
            await database.CloseConnection();
        }
    }
    
    private static async Task SendAlert(WSConnection connection, AlertType type, string message)
    {
        AlertData alertData = new()
        {
            Type = type,
            Message = message
        };

        AlertMessage alertMessage = new(alertData);
        await alertMessage.SendTo(connection);
    }
}

