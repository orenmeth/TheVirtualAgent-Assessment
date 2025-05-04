using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class TransactionRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider, IDapperWrapper dapperWrapper) : ITransactionRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;
        private readonly IDapperWrapper _dapperWrapper = dapperWrapper;

        public async Task<TransactionDto?> GetTransactionAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            var commandDefinition = new CommandDefinition(
                commandText: "GetTransaction",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            return await _dapperWrapper.QuerySingleOrDefaultAsync<TransactionDto>(connection, commandDefinition);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByAccountCodeAsync(int accountCode, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@account_code", accountCode);

            var commandDefinition = new CommandDefinition(
                commandText: "GetTransactionsByAccountCode",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            return await _dapperWrapper.QueryAsync<TransactionDto>(connection, commandDefinition);
        }

        public async Task<int> UpsertTransactionAsync(TransactionDto transaction, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", transaction.Code);
            parameters.Add("@account_code", transaction.Account_Code);
            parameters.Add("@transaction_date", transaction.Transaction_Date);
            parameters.Add("@amount", transaction.Amount);
            parameters.Add("@description", transaction.Description);
            parameters.Add("@RETURN_CODE", DbType.Int32, direction: ParameterDirection.Output);

            var commandDefinition = new CommandDefinition(
                commandText: "UpsertTransaction",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await _dapperWrapper.ExecuteAsync(connection, commandDefinition);
            var returnCode = parameters.Get<int>("@RETURN_CODE");

            return returnCode;
        }

        public async Task DeleteTransactionAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            var commandDefinition = new CommandDefinition(
                commandText: "DeleteTransaction",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await _dapperWrapper.ExecuteAsync(connection, commandDefinition);
        }
    }
}
