using System.Data;
using Npgsql;

namespace Throne.Shared.Database;

public class Database
{
  private NpgsqlConnection? connection;
  private readonly string connectionString;
  private readonly string host = "127.0.0.1";
  private readonly string username = "postgres";
  private readonly string password = "postgres";
  private readonly string database = "";

  public Database()
  {
    connectionString = $"Host={host};Username={username};Password={password};Database={database};";
  }

  private async Task OpenConnection()
  {
    if (connection == null || connection.State == ConnectionState.Closed)
    {
      connection = new NpgsqlConnection(connectionString);
      await connection.OpenAsync();
    }
  }

  public async Task CloseConnection()
  {
    if (connection != null && connection.State == ConnectionState.Open)
    {
      await connection.CloseAsync();
      await connection.DisposeAsync();
    }
  }

  public async Task<List<T>> ExecutarConsultaAsync<T>(string query, Func<IDataReader, T> map, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    using (var cmd = new NpgsqlCommand(query, connection))
    {
      if (parameters != null)
      {
        cmd.Parameters.AddRange(parameters);
      }

      using (var reader = await cmd.ExecuteReaderAsync())
      {
        var results = new List<T>();
        while (await reader.ReadAsync())
        {
          results.Add(map(reader));
        }
        return results;
      }
    }
  }

  public async Task<int> ExecutarAtualizacaoAsync(string query, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    using (var cmd = new NpgsqlCommand(query, connection))
    {
      if (parameters != null)
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
  }

  public async Task ExecutarProcedureAsync(string procedureName, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    using (var cmd = new NpgsqlCommand(procedureName, connection))
    {
      cmd.CommandType = CommandType.StoredProcedure;

      if (parameters != null)
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
  }

  public async Task<T?> ExecutarFunctionAsync<T>(string functionName, params NpgsqlParameter[] parameters)
  {
    await OpenConnection();

    using (var cmd = new NpgsqlCommand(functionName, connection))
    {
      cmd.CommandType = CommandType.StoredProcedure;

      if (parameters != null)
      {
        cmd.Parameters.AddRange(parameters);
      }

      try
      {
        var result = await cmd.ExecuteScalarAsync();

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

}
