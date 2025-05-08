using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface ITransactionService
    {
        Task DeleteTransactionAsync(int code, CancellationToken cancellationToken);
        Task<TransactionResponse> GetTransactionAsync(int code, CancellationToken cancellationToken);
        Task<List<TransactionResponse>> GetTransactionsByAccountCodeAsync(int accountCode, CancellationToken cancellationToken);
        Task<TransactionResponse> UpsertTransactionAsync(TransactionRequest transaction, CancellationToken cancellationToken);
    }
}