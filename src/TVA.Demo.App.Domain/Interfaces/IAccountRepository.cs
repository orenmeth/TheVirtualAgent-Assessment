using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task DeleteAccountAsync(int code, CancellationToken cancellationToken);
        Task<AccountDto?> GetAccountAsync(int code, CancellationToken cancellationToken);
        Task<IEnumerable<AccountDto>> GetAccountsByPersonCodeAsync(int personCode, CancellationToken cancellationToken);
        Task<int> UpsertAccountAsync(AccountDto account, CancellationToken cancellationToken);
    }
}