using Dapper;
using Microsoft.Data.SqlClient;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;

namespace TVA.Demo.App.Infrastructure.Repositories
{
    public class UserRepository(IConnectionFactory connectionFactory, IDbConnectionProvider dbConnectionProvider, IDapperWrapper dapperWrapper) : IUserRepository
    {
        private readonly IConnectionFactory _connectionFactory = connectionFactory;
        private readonly IDbConnectionProvider _dbConnectionProvider = dbConnectionProvider;
        private readonly IDapperWrapper _dapperWrapper = dapperWrapper;

        public async Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var sql = "SELECT * FROM Users WHERE Username = @UsernameOrEmail OR Email = @UsernameOrEmail";
            var parameters = new { UsernameOrEmail = usernameOrEmail };

            return await _dapperWrapper.QuerySingleOrDefaultAsync<User>(connection, sql, parameters);
        }

        public async Task<User?> CreateUser(User user, CancellationToken cancellationToken)
        {
            using SqlConnection connection = await _connectionFactory.CreateSqlConnectionAsync(_dbConnectionProvider.GetDefaultDbConnection(), cancellationToken);
            var sql = @"INSERT INTO Users (Username, Email, PasswordHash, DateCreated, IsActive)
                            OUTPUT INSERTED.Id, INSERTED.Username, INSERTED.Email, INSERTED.PasswordHash, INSERTED.DateCreated, INSERTED.IsActive
                            VALUES (@Username, @Email, @PasswordHash, @DateCreated, @IsActive);";

            return (await connection.QuerySingleOrDefaultAsync<User>(sql, user));
        }
    }
}