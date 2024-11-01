using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace RecommendationsAppApiLibrary.DataAccess;

public class SqliteDataAccess : ISqliteDataAccess
{
    private readonly IConfiguration _config;

    public SqliteDataAccess(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<T>> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using IDbConnection connection = new SqliteConnection(connectionString);

        var rows = await connection.QueryAsync<T>(sqlStatement, parameters);

        return rows.ToList();

    }

    public async Task SaveData<T>(string sqlStatement, T parameters, string connectionStringName)
    {
        string connectionString = _config.GetConnectionString(connectionStringName);

        using IDbConnection connection = new SqliteConnection(connectionString);

        await connection.ExecuteAsync(sqlStatement, parameters);

    }
}