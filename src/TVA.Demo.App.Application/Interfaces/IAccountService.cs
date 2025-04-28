using TVA.Demo.App.Domain.Models;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetAccountAsync(int code, CancellationToken cancellationToken);
        Task<List<Account>> GetAccountsByPersonCodeAsync(int personCode, CancellationToken cancellationToken);
    }
}