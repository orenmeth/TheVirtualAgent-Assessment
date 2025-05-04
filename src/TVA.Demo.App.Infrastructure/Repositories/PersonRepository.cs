using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class PersonRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider) : IPersonRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;

        public async Task<IEnumerable<PersonDto>> GetPersonsAsync(CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            return await connection.QueryAsync<PersonDto>("GetPersons", commandType: CommandType.StoredProcedure);
        }

        public async Task<PersonDto?> GetPersonAsync(int code, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);

            return await connection.QuerySingleOrDefaultAsync<PersonDto>("GetPerson", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task UpsertPersonAsync(PersonDto person, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", person.Code);
            parameters.Add("@name", person.Name);
            parameters.Add("@last_name", person.Surname);
            parameters.Add("@id_number", person.Id_Number);

            await connection.ExecuteAsync("UpsertPerson", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task DeletePersonAsync(int code, bool deleteRelatedAccounts, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);
            parameters.Add("@delete_related_accounts", deleteRelatedAccounts);

            await connection.ExecuteAsync("DeletePerson", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
