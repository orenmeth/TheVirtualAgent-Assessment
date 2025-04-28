using Microsoft.Data.SqlClient;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IConnectionFactory
    {
        Task<SqlConnection> CreateSqlConnectionAsync(SqlConnection connection, CancellationToken cancellationToken = default);
    }
}