using Npgsql;
using Throne.Server.Websocket.Communication.Outgoing.Messages;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Server.Websocket.Core;
using Throne.Shared.Database;
using Throne.Shared.VersionsChecker;
using BC = BCrypt.Net.BCrypt;

namespace Throne.Server.Websocket.Communication.Incoming.Requests;

public class SignUpRequest : IIncoming
{
    private readonly IDatabase database;
    private readonly VersionChecker versionChecker;

    public SignUpRequest()
    {
        IServiceProvider serviceProvider = ServiceLocator.GetServiceProvider();
        database = serviceProvider.GetRequiredService<IDatabase>();
        versionChecker = serviceProvider.GetRequiredService<VersionChecker>();
    }

    public async Task Handle(WsConnection connection, ClientMessage clientMessage)
    {
        string email = clientMessage.GetString();
        string password = clientMessage.GetString();
        string rePassword = clientMessage.GetString();
        int major = clientMessage.GetInt16();
        int minor = clientMessage.GetInt16();
        int revision = clientMessage.GetInt16();

        bool versionValid = versionChecker.Check(major, minor, revision);
        if (!versionValid)
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

        if (password != rePassword)
        {
            AlertData alertData = new()
            {
                Type = AlertType.Error,
                Message = "As senhas fornecidas não coincidem. Por favor, tente novamente."
            };

            AlertMessage alertMessage = new(alertData);
            await alertMessage.SendTo(connection);

            return;
        }

        try
        {
            string hashPassword = BC.HashPassword(password);

            NpgsqlParameter[] parameters =
            [
                new NpgsqlParameter("@p_email", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = email },
                new NpgsqlParameter("@p_password", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = hashPassword },
                new NpgsqlParameter("@p_character_count", NpgsqlTypes.NpgsqlDbType.Integer) { Value = 2 },
                new NpgsqlParameter("@p_coins", NpgsqlTypes.NpgsqlDbType.Integer) { Value = 0 },
            ];

            await database.ExecuteProcedure("insert_account", parameters);

            AlertData successAlertData = new()
            {
                Type = AlertType.Info,
                Message = "Sua conta foi criada com sucesso!"
            };

            AlertMessage successAlertMessage = new(successAlertData);
            await successAlertMessage.SendTo(connection);
        }
        catch (Exception e)
        {
            AlertData errorAlertData = new()
            {
                Type = AlertType.Error,
                Message = $"Ocorreu um erro ao criar a conta: {e.Message}"
            };

            AlertMessage errorAlertMessage = new(errorAlertData);
            await errorAlertMessage.SendTo(connection);
        }
        finally
        {
            await database.CloseConnection();
        }
    }
}