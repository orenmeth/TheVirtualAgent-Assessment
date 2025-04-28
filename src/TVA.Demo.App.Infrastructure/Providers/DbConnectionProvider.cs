using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Providers
{
    public class DbConnectionProvider(IConfiguration configuration) : IDbConnectionProvider
    {
        private readonly IConfiguration _configuration = configuration;

        public SqlConnection GetDefaultDbConnection()
        {
            string? defaultDbConnectionString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(defaultDbConnectionString);
        }
    }
}
