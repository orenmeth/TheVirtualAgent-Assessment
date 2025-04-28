using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task DeleteTransactionAsync(int code);
        Task<TransactionDto?> GetTransactionAsync(int code);
        Task<IEnumerable<TransactionDto>> GetTransactionsByAccountCodeAsync(int accountCode);
        Task UpsertTransactionAsync(int? code, int accountCode, DateTime transactionDate, DateTime captureDate, decimal amount, string description);
    }
}