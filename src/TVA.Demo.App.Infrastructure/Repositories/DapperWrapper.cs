using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage(Justification = "No need to test Dapper's implementation.")]
    public class DapperWrapper(ILogger<DapperWrapper> logger) : IDapperWrapper
    {
        private readonly ILogger<DapperWrapper> _logger = logger;

        public async Task<int> ExecuteAsync(IDbConnection connection, CommandDefinition command)
        {
            try
            {
                return await connection.ExecuteAsync(command);
            }
            catch (SqlException se)
            {
                _logger.LogError(se, se.Message);
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public async Task<int> ExecuteAsync(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(IDbConnection connection, CommandDefinition command)
        {
            return await connection.QuerySingleOrDefaultAsync<T>(command);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, CommandDefinition command)
        {
            return await connection.QueryAsync<T>(command);
        }
    }
}
