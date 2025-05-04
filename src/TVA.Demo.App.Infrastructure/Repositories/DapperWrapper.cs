using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    [ExcludeFromCodeCoverage(Justification = "No need to test Dapper's implementation.")]
    public class DapperWrapper : IDapperWrapper
    {
        public async Task<int> ExecuteAsync(IDbConnection connection, CommandDefinition command)
        {
            return await connection.ExecuteAsync(command);
        }

        public async Task<int> ExecuteAsync(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }
    }
}
