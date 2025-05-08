using Microsoft.Data.SqlClient;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IDbConnectionProvider
    {
        SqlConnection GetDefaultDbConnection();
    }
}