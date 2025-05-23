﻿using Dapper;
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

            var accounts = await _dapperWrapper.QueryAsync<AccountDto>(connection, commandDefinition);
            return accounts ?? [];
        }

        public async Task<int> UpsertAccountAsync(AccountDto account, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", account.Code, DbType.Int32);
            parameters.Add("@person_code", account.Person_Code, DbType.Int32);
            parameters.Add("@account_number", account.Account_Number, DbType.String);
            parameters.Add("@outstanding_balance", account.Outstanding_Balance, DbType.Decimal);
            parameters.Add("@account_status_id", account.Account_Status_Id, DbType.Int32);
            parameters.Add("@RETURN_CODE", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var commandDefinition = new CommandDefinition(
                commandText: "UpsertAccount",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await _dapperWrapper.ExecuteAsync(connection, commandDefinition);
            int returnCode = parameters.Get<int>("@RETURN_CODE");
            return returnCode;
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
