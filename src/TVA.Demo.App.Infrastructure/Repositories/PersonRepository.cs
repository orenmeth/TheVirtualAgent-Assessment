using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class PersonRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider, IDapperWrapper dapperWrapper) : IPersonRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;
        private readonly IDapperWrapper _dapperWrapper = dapperWrapper;

        public async Task<IEnumerable<PersonDto>> GetPersonsAsync(CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);

            var commandDefinition = new CommandDefinition(
                commandText: "GetPersons",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            return await _dapperWrapper.QueryAsync<PersonDto>(connection, commandDefinition);
        }

        public async Task<PersonDto?> GetPersonAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            var commandDefinition = new CommandDefinition(
                commandText: "GetPerson",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            return await _dapperWrapper.QuerySingleOrDefaultAsync<PersonDto>(connection, commandDefinition);
        }

        public async Task<int> UpsertPersonAsync(PersonDto person, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", person.Code, DbType.Int32);
            parameters.Add("@first_name", person.Name, DbType.String);
            parameters.Add("@last_name", person.Surname, DbType.String);
            parameters.Add("@id_number", person.Id_Number, DbType.String);
            parameters.Add("@RETURN_CODE", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var commandDefinition = new CommandDefinition(
                commandText: "UpsertPerson",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await _dapperWrapper.ExecuteAsync(connection, commandDefinition);
            int returnCode = parameters.Get<int>("@RETURN_CODE");
            return returnCode;
        }

        public async Task DeletePersonAsync(int code, bool deleteRelatedAccounts, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code, DbType.Int32);
            parameters.Add("@delete_related_accounts_and_transactions", deleteRelatedAccounts, DbType.Boolean);

            var commandDefinition = new CommandDefinition(
                commandText: "DeletePerson",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            await _dapperWrapper.ExecuteAsync(connection, commandDefinition);
        }
    }
}
