using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface IAccountService
    {
        Task DeleteAccountAsync(int code, CancellationToken cancellationToken);
        Task<AccountResponse> GetAccountAsync(int code, CancellationToken cancellationToken);
        Task<List<AccountResponse>> GetAccountsByPersonCodeAsync(int personCode, CancellationToken cancellationToken);
        Task<AccountResponse> UpsertAccountAsync(AccountRequest account, CancellationToken cancellationToken);
    }
}