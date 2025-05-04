using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task DeleteTransactionAsync(int code, CancellationToken cancellationToken);
        Task<TransactionDto?> GetTransactionAsync(int code, CancellationToken cancellationToken);
        Task<IEnumerable<TransactionDto>> GetTransactionsByAccountCodeAsync(int accountCode, CancellationToken cancellationToken);
        Task UpsertTransactionAsync(TransactionDto transaction, CancellationToken cancellationToken);
    }
}