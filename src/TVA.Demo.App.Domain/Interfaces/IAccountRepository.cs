using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task DeleteAccountAsync(int code, CancellationToken cancellationToken);
        Task<AccountDto?> GetAccountAsync(int code, CancellationToken cancellationToken);
        Task<IEnumerable<AccountDto>> GetAccountsByPersonCodeAsync(int personCode, CancellationToken cancellationToken);
        Task UpsertAccountAsync(int? code, int personCode, string accountNumber, string accountType, decimal balance, CancellationToken cancellationToken);
    }
}