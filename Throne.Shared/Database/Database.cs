using System.Data;
using Npgsql;

namespace Throne.Shared.Database;

public class Database
{
  private NpgsqlConnection? connection;
  private readonly string connectionString;

  public Database()
  {
    connectionString = "Host=127.0.0.1;Username=postgres;Password=postgres;Database=throne;";
  }

  private async Task OpenConnectionAsync()
  {
    if (connection == null || connection.State == ConnectionState.Closed)
    {
      connection = new NpgsqlConnection(connectionString);
      await connection.OpenAsync();
    }
  }

  public async Task CloseConnectionAsync()
  {
    if (connection != null && connection.State == ConnectionState.Open)
    {
      await connection.CloseAsync();
      await connection.DisposeAsync();
      connection = null;
    }
  }

  public async Task<List<T>> ExecutarConsultaAsync<T>(string query, Func<IDataReader, T> map, params NpgsqlParameter[] parameters)
  {
    await OpenConnectionAsync();

    using (var cmd = new NpgsqlCommand(query, connection))
    {
      if (parameters.Length > 0)
      {
        cmd.Parameters.AddRange(parameters);
      }

      var results = new List<T>();
      try
      {
        using (var reader = await cmd.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            results.Add(map(reader));
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Erro ao executar a consulta: {ex.Message}");
      }

      return results;
    }
  }

  public async Task<int> ExecutarAtualizacaoAsync(string query, params NpgsqlParameter[] parameters)
  {
    await OpenConnectionAsync();

    using (var cmd = new NpgsqlCommand(query, connection))
    {
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
  }

  public async Task ExecutarProcedureAsync(string procedureName, params NpgsqlParameter[] parameters)
  {
    await OpenConnectionAsync();

    using (var cmd = new NpgsqlCommand(procedureName, connection))
    {
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
  }

  public async Task<T?> ExecutarFunctionAsync<T>(string functionName, params NpgsqlParameter[] parameters)
  {
    await OpenConnectionAsync();

    using (var cmd = new NpgsqlCommand(functionName, connection))
    {
      cmd.CommandType = CommandType.Text;

      if (parameters.Length > 0)
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
