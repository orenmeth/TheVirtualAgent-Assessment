using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;
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

        public async Task UpsertTransactionAsync(TransactionDto transaction, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", transaction.Code);
            parameters.Add("@account_code", transaction.Account_Code);
            parameters.Add("@transaction_date", transaction.Transaction_Date);
            parameters.Add("@capture_date", transaction.Capture_Date);
            parameters.Add("@amount", transaction.Amount);
            parameters.Add("@description", transaction.Description);

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
