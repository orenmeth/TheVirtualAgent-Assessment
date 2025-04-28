using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Factories
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        public async Task<SqlConnection> CreateSqlConnectionAsync(SqlConnection connection, CancellationToken cancellationToken = default)
        {
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            return connection;
        }
    }
}
