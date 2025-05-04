using Azure;
using Microsoft.Extensions.Caching.Memory;
using TVA.Demo.App.Application.Extensions;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Domain.Models;

namespace TVA.Demo.App.Application.Services
{
    public class TransactionService(ITransactionRepository transactionRepository, IMemoryCache cache) : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository = transactionRepository;
        private readonly IMemoryCache _cache = cache;
        private const string TransactionsCacheKey = "TransactionsData";
        private const string TransactionCacheKey = "TransactionData";

        public async Task<List<Transaction>> GetTransactionsByAccountCodeAsync(int accountCode, CancellationToken cancellationToken)
        {
            string cacheKey = $"{TransactionsCacheKey}_AccountCode_{accountCode}";

            IEnumerable<TransactionDto>? transactionDtos;
            if (_cache.TryGetValue(cacheKey, out List<TransactionDto>? cachedTransactions))
            {
                transactionDtos = cachedTransactions;
            }
            else
            {
                transactionDtos = await _transactionRepository.GetTransactionsByAccountCodeAsync(accountCode, cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, transactionDtos, cacheEntryOptions);
            }

            if (transactionDtos == null || !transactionDtos.Any())
            {
                return [];
            }

            var transactions = transactionDtos
                .Select(t => new Transaction
                {
                    Code = t.Code,
                    AccountCode = t.Account_Code,
                    TransactionDate = t.Transaction_Date,
                    CaptureDate = t.Capture_Date,
                    Amount = t.Amount,
                    Description = t.Description!
                })
                .ToList();

            return transactions;
        }

        public async Task<Transaction> GetTransactionAsync(int code, CancellationToken cancellationToken)
        {
            string transactionCacheKey = $"{TransactionCacheKey}_Code_{code}";

            TransactionDto? transactionDto;
            if (_cache.TryGetValue(transactionCacheKey, out TransactionDto? cachedTransaction))
            {
                transactionDto = cachedTransaction;
            }
            else
            {
                transactionDto = await _transactionRepository.GetTransactionAsync(code, cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(transactionCacheKey, transactionDto, cacheEntryOptions);
            }

            if (transactionDto == null)
            {
                throw new RequestFailedException($"Transaction with code {code} not found.");
            }

            var transaction = new Transaction
            {
                Code = transactionDto.Code,
                AccountCode = transactionDto.Account_Code,
                TransactionDate = transactionDto.Transaction_Date,
                CaptureDate = transactionDto.Capture_Date,
                Amount = transactionDto.Amount,
                Description = transactionDto.Description!
            };

            return transaction;
        }

        private void InvalidateTransactionsCache()
        {
            var keysToRemove = _cache.GetKeysStartingWith(TransactionsCacheKey).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }

        public async Task DeleteTransactionAsync(int code, CancellationToken cancellationToken)
        {
            string transactionCacheKey = $"{TransactionCacheKey}_Code_{code}";
            _cache.Remove(transactionCacheKey);
            InvalidateTransactionsCache();

            await _transactionRepository.DeleteTransactionAsync(code, cancellationToken);
        }

        public async Task<Transaction> UpsertTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
        {
            TransactionDto transactionDto = new()
            {
                Code = transaction.Code,
                Account_Code = transaction.AccountCode,
                Transaction_Date = transaction.TransactionDate,
                Capture_Date = transaction.CaptureDate,
                Amount = transaction.Amount,
                Description = transaction.Description!
            };

            await _transactionRepository.UpsertTransactionAsync(transactionDto, cancellationToken);

            InvalidateTransactionsCache();
            string transactionCacheKey = $"{TransactionCacheKey}_Code_{transaction.Code}";
            _cache.Remove(transactionCacheKey);

            return await GetTransactionAsync(transaction.Code, cancellationToken);
        }
    }
}
