using Npgsql;
using Throne.Server.Websocket.Communication.Outgoing.Messages;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Server.Websocket.Core;
using Throne.Shared.Database;

namespace Throne.Server.Websocket.Communication.Incoming.Requests;

public class DeleteCharRequest : IIncoming
{
    private readonly IDatabase database;
    
    public DeleteCharRequest()
    {
        IServiceProvider serviceProvider = ServiceLocator.GetServiceProvider();
        database = serviceProvider.GetRequiredService<IDatabase>();
    }
    
    public async Task Handle(WsConnection connection, ClientMessage clientMessage)
    {
        int characterId = clientMessage.GetInt32();
        int accountId = clientMessage.GetInt32();

        try
        {
            NpgsqlParameter[] parameters =
            [
                new NpgsqlParameter("@p_character_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = characterId },
                new NpgsqlParameter("@p_account_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = accountId },
            ];
            
            await database.ExecuteProcedure("delete_character", parameters);
            
            AlertData successAlertData = new()
            {
                Type = AlertType.Info,
                Message = "Seu personagem foi apagado!"
            };
        
            AlertMessage successAlertMessage = new(successAlertData);
            await successAlertMessage.SendTo(connection);
        }
        catch (Exception e)
        {
            AlertData errorAlertData = new()
            {
                Type = AlertType.Error,
                Message = $"Ocorreu um erro ao apagar o personagem: {e.Message}"
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
