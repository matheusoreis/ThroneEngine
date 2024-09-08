using System.Data;
using Npgsql;

namespace Throne.Shared.Database;

public class Database : IDatabase
{
  private NpgsqlConnection? connection;
  private const string ConnectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=throne;";
  
  private async Task OpenConnection()
  {
    if (connection == null || connection.State == ConnectionState.Closed)
    {
      connection = new NpgsqlConnection(ConnectionString);
      await connection.OpenAsync();
    }
  }

  public Task OpenOpenConnection()
  {
    throw new NotImplementedException();
  }

  public async Task CloseConnection()
  {
    if (connection is { State: ConnectionState.Open })
    {
      await connection.CloseAsync();
      await connection.DisposeAsync();
      connection = null;
    }
  }

  public async Task<List<T>> RunQuery<T>(string query, Func<IDataReader, T> map, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    await using NpgsqlCommand cmd = new (query, connection);
    if (parameters.Length > 0)
    {
      cmd.Parameters.AddRange(parameters);
    }

    List<T> results = [];
    try
    {
      await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
      while (await reader.ReadAsync())
      {
        results.Add(map(reader));
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Erro ao executar a consulta: {ex.Message}");
    }

    return results;
  }

  public async Task<int> ExecuteUpdate(string query, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    await using NpgsqlCommand cmd = new (query, connection);
    if (parameters.Length > 0)
    {
      cmd.Parameters.AddRange(parameters);
    }

    try
    {
      return await cmd.ExecuteNonQueryAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Erro ao executar a atualização: {ex.Message}");
      return -1;
    }
  }

  public async Task ExecuteProcedure(string procedureName, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    await using NpgsqlCommand cmd = new (procedureName, connection);
    cmd.CommandType = CommandType.StoredProcedure;

    if (parameters.Length > 0)
    {
      cmd.Parameters.AddRange(parameters);
    }

    try
    {
      await cmd.ExecuteNonQueryAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Erro ao executar a procedure: {ex.Message}");
    }
  }

  public async Task<T?> ExecuteFunction<T>(string functionName, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    await using NpgsqlCommand cmd = new (functionName, connection);
    cmd.CommandType = CommandType.Text;

    if (parameters.Length > 0)
    {
      cmd.Parameters.AddRange(parameters);
    }

    try
    {
      object? result = await cmd.ExecuteScalarAsync();

      if (result == null || result == DBNull.Value)
      {
        return default;
      }

      return (T)result;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Erro ao executar a função: {ex.Message}");
      return default;
    }
  }
}
