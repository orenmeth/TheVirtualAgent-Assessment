using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateUserAsync(string usernameOrEmail, string password, CancellationToken cancellationToken);
        Task<(bool Success, string Message, User User)> RegisterUserAsync(string username, string email, string password, CancellationToken cancellationToken);
    }
}