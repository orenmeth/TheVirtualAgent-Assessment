using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IAccountStatusRepository
    {
        Task<IEnumerable<AccountStatusDto>> GetAccountStatusesAsync(CancellationToken cancellationToken);
    }
}