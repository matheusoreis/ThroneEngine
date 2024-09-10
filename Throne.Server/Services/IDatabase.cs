using System.Data;
using Npgsql;

namespace Throne.Server.Services;

public interface IDatabase
{
    Task CloseConnection();
    Task<List<T>> RunQuery<T>(string query, Func<IDataReader, T> map, params NpgsqlParameter[] parameters);
    Task<int> ExecuteUpdate(string query, params NpgsqlParameter[] parameters);
    Task ExecuteProcedure(string procedureName, params NpgsqlParameter[] parameters);
    Task<T?> ExecuteFunction<T>(string functionName, params NpgsqlParameter[] parameters);
}