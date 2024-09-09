using Npgsql;
using Throne.Server.Websocket.Communication.Outgoing.Messages;
using Throne.Server.Websocket.Communication.Protocol;
using Throne.Server.Websocket.Core;
using Throne.Server.Websocket.Core.Data;
using Throne.Shared.Database;
using Throne.Shared.Logger;

namespace Throne.Server.Websocket.Communication.Incoming.Requests;

public class CharListRequest : IIncoming
{
    private readonly IDatabase database;

    public CharListRequest()
    {
        IServiceProvider serviceProvider = ServiceLocator.GetServiceProvider();
        database = serviceProvider.GetRequiredService<IDatabase>();
    }

    public async Task Handle(WSConnection connection, ClientMessage clientMessage)
    {
        int accountId = clientMessage.GetInt32();

        try
        {
            const string getCharactersQuery = "SELECT id, name, color, gender, account_id FROM character_list WHERE account_id = @p_account_id";
            NpgsqlParameter[] parameters =
            [
                new NpgsqlParameter("@p_account_id", accountId)
            ];

            List<CharacterData> characters = await database.RunQuery(getCharactersQuery, reader => new CharacterData
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                AccountId = reader.GetInt32(reader.GetOrdinal("account_id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Color = reader.GetString(reader.GetOrdinal("color")),
                Gender = reader.GetString(reader.GetOrdinal("gender")),
            }, parameters);

            const string getCharacterCountQuery = "SELECT character_count FROM accounts WHERE id = @p_account_id";
            
            List<int> characterCountList = await database.RunQuery<int>(
                getCharacterCountQuery, 
                reader => reader.GetInt32(0), 
                new NpgsqlParameter("@p_account_id", accountId)
            );

            int characterCount = characterCountList.FirstOrDefault();
            
            CharListMessage charListMessage = new(characters, characterCount);
            await charListMessage.SendTo(connection);
        }
        catch (Exception ex)
        {
            Logger.Error($"Erro ao buscar personagens: {ex.Message}");
            await SendAlert(connection, AlertType.Error, "Erro ao buscar personagens.");
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