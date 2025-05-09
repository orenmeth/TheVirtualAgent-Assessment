using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> CreateUser(User user, CancellationToken cancellationToken);
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail, CancellationToken cancellationToken);
    }
}