using Dapper;
using System.Data;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IDapperWrapper
    {
        Task<int> ExecuteAsync(IDbConnection connection, CommandDefinition command);
        Task<int> ExecuteAsync(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, CommandDefinition command);
        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, CommandDefinition command);
        Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, string sql, object parameters);
    }
}