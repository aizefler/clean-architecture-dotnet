using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace TodoApp.Infrastructure.Data.SqlServer.Common.Queries;

[ExcludeFromCodeCoverage]
public abstract class BaseQueryRepository
{
    protected readonly string _connectionString;
    protected readonly IDbConnection _connection;

    protected BaseQueryRepository(IConfiguration configuration, string connectionStringName = "DefaultConnection")
    {
        _connectionString = configuration.GetConnectionString(connectionStringName) 
            ?? throw new ArgumentNullException(nameof(connectionStringName), "Connection string not found");
        _connection = new SqlConnection(_connectionString);
    }

    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    protected async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    protected async Task<T> QueryFirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    protected async Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, commandType);
    }

    protected async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }

    protected async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
    }

    protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, TReturn>(string sql, Func<T1, T2, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, TReturn>(string sql, Func<T1, T2, T3, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, TReturn>(string sql, Func<T1, T2, T3, T4, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, T5, TReturn>(string sql, Func<T1, T2, T3, T4, T5, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, T5, T6, TReturn>(string sql, Func<T1, T2, T3, T4, T5, T6, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    protected async Task<IEnumerable<TReturn>> QueryAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(string sql, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map, object? param = null, IDbTransaction? transaction = null, bool buffered = true, string? splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return await _connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
} 