using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class AccountRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider) : IAccountRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;

        public async Task UpsertAccountAsync(int? code, int personCode, string accountNumber, string accountType, decimal balance, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);
            parameters.Add("@person_code", personCode);
            parameters.Add("@account_number", accountNumber);
            parameters.Add("@account_type", accountType);
            parameters.Add("@balance", balance);

            await connection.ExecuteAsync("UpsertAccount", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAccountAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            await connection.ExecuteAsync("DeleteAccount", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<AccountDto?> GetAccountAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            return await connection.QuerySingleOrDefaultAsync<AccountDto>("GetAccount", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<AccountDto>> GetAccountsByPersonCodeAsync(int personCode, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@person_code", personCode);
            return await connection.QueryAsync<AccountDto>("GetAccountByPersonCode", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
