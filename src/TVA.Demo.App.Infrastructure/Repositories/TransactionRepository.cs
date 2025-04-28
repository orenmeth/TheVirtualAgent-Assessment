using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class TransactionRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider) : ITransactionRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;

        public async Task<TransactionDto?> GetTransactionAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            return await connection.QuerySingleOrDefaultAsync<TransactionDto>("GetTransaction", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByAccountCodeAsync(int accountCode, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@account_code", accountCode);

            return await connection.QueryAsync<TransactionDto>("GetTransactionsByAccountCode", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpsertTransactionAsync(int? code, int accountCode, DateTime transactionDate, DateTime captureDate, decimal amount, string description, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);
            parameters.Add("@account_code", accountCode);
            parameters.Add("@transaction_date", transactionDate);
            parameters.Add("@capture_date", captureDate);
            parameters.Add("@amount", amount);
            parameters.Add("@description", description);

            await connection.ExecuteAsync("UpsertTransaction", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteTransactionAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            await connection.ExecuteAsync("DeleteTransaction", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
