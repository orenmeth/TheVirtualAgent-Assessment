using TVA.Demo.App.Domain.Models;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface ITransactionService
    {
        Task DeleteTransactionAsync(int code, CancellationToken cancellationToken);
        Task<Transaction> GetTransactionAsync(int code, CancellationToken cancellationToken);
        Task<List<Transaction>> GetTransactionsByAccountCodeAsync(int accountCode, CancellationToken cancellationToken);
        Task<Transaction> UpsertTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
    }
}