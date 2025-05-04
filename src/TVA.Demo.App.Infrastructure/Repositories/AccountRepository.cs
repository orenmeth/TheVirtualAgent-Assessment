using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class AccountRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider, IDapperWrapper dapperWrapper) : IAccountRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;
        private readonly IDapperWrapper _dapperWrapper = dapperWrapper;

        public async Task<AccountDto?> GetAccountAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            var commandDefinition = new CommandDefinition(
                commandText: "GetAccount",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            return await _dapperWrapper.QuerySingleOrDefaultAsync<AccountDto>(connection, commandDefinition);
        }

        public async Task<IEnumerable<AccountDto>> GetAccountsByPersonCodeAsync(int personCode, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@person_code", personCode);

            var commandDefinition = new CommandDefinition(
                commandText: "GetAccountsByPersonCode",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            return await _dapperWrapper.QueryAsync<AccountDto>(connection, commandDefinition);
        }

        public async Task UpsertAccountAsync(AccountDto account, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", account.Code);
            parameters.Add("@person_code", account.Person_Code);
            parameters.Add("@account_number", account.Account_Number);
            parameters.Add("@outstanding_balance", account.Outstanding_Balance);

            var commandDefinition = new CommandDefinition(
                commandText: "UpsertAccount",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await _dapperWrapper.ExecuteAsync(connection, commandDefinition);
        }

        public async Task DeleteAccountAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            var commandDefinition = new CommandDefinition(
                commandText: "DeleteAccount",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await _dapperWrapper.ExecuteAsync(connection, commandDefinition);
        }
    }
}
