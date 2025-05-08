using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class AccountStatusRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider, IDapperWrapper dapperWrapper) : IAccountStatusRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;
        private readonly IDapperWrapper _dapperWrapper = dapperWrapper;

        public async Task<IEnumerable<AccountStatusDto?>> GetAccountStatusesAsync(CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var commandDefinition = new CommandDefinition(
                commandText: "GetAccountStatuses",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            var accountStatuses = await _dapperWrapper.QueryAsync<AccountStatusDto>(connection, commandDefinition);
            return accountStatuses ?? [];
        }
    }
}
