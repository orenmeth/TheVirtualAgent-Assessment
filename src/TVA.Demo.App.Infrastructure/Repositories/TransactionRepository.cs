using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class TransactionRepository(string connectionString) : ITransactionRepository
    {
        private readonly string _connectionString = connectionString;

        public async Task<TransactionDto?> GetTransactionAsync(int code)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            return await db.QuerySingleOrDefaultAsync<TransactionDto>("GetTransaction", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByAccountCodeAsync(int accountCode)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@account_code", accountCode);

            return await db.QueryAsync<TransactionDto>("GetTransactionsByAccountCode", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpsertTransactionAsync(int? code, int accountCode, DateTime transactionDate, DateTime captureDate, decimal amount, string description)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);
            parameters.Add("@account_code", accountCode);
            parameters.Add("@transaction_date", transactionDate);
            parameters.Add("@capture_date", captureDate);
            parameters.Add("@amount", amount);
            parameters.Add("@description", description);

            await db.ExecuteAsync("UpsertTransaction", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteTransactionAsync(int code)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            await db.ExecuteAsync("DeleteTransaction", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
