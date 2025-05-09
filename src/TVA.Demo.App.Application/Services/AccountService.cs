using Azure;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Application.Services
{
    public class AccountService(IAccountStatusRepository accountStatusRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, IMemoryCache cache) : IAccountService
    {
        private readonly IAccountStatusRepository _accountStatusRepository = accountStatusRepository;
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly ITransactionRepository _transactionRepository = transactionRepository;
        private readonly IMemoryCache _cache = cache;

        private const string AccountCacheKey = "AccountData";
        private const string AccountTransactionsCacheKey = "AccountTransactionsData";
        private const string PersonAccountsCacheKey = "PersonAccountsData";
        private const string AccountStatusCacheKey = "AccountStatusData";

        public async Task<List<AccountResponse>> GetAccountsByPersonCodeAsync(int personCode, CancellationToken cancellationToken)
        {
            string cacheKey = $"{PersonAccountsCacheKey}_Code_{personCode}";

            IEnumerable<AccountDto> accountDtos = await GetPersonAccountDtos(cacheKey, personCode, cancellationToken);
            if (accountDtos == null || !accountDtos.Any())
            {
                return [];
            }

            var accounts = accountDtos
                .Select(a => new AccountResponse
                {
                    Code = a.Code,
                    PersonCode = a.Person_Code,
                    AccountNumber = a.Account_Number,
                    OutstandingBalance = a.Outstanding_Balance,
                    Transactions = []
                })
                .ToList();

            return accounts;
        }

        public async Task<AccountResponse> GetAccountAsync(int code, CancellationToken cancellationToken)
        {
            string accountCacheKey = $"{AccountCacheKey}_Code_{code}";
            string accountTransactionsCacheKey = $"{AccountTransactionsCacheKey}_Code_{code}";

            AccountDto accountDto = await GetAccountDtoAsync(accountCacheKey, code, cancellationToken);

            IEnumerable<TransactionDto> transactionDtos = await GetAccountTransactionDtos(accountTransactionsCacheKey, code, cancellationToken);

            var transactions = transactionDtos
                .Select(t => new TransactionResponse
                {
                    Code = t.Code,
                    AccountCode = t.Account_Code,
                    TransactionDate = t.Transaction_Date,
                    CaptureDate = t.Capture_Date,
                    Amount = t.Amount,
                    Description = t.Description
                })
                .ToList();

            AccountResponse account = new()
            {
                Code = accountDto.Code,
                PersonCode = accountDto.Person_Code,
                AccountNumber = accountDto.Account_Number,
                OutstandingBalance = accountDto.Outstanding_Balance,
                AccountStatusId = accountDto.Account_Status_Id,
                Transactions = transactions
            };

            return account;
        }

        public async Task DeleteAccountAsync(int code, CancellationToken cancellationToken)
        {
            var accountDto = await _accountRepository.GetAccountAsync(code, cancellationToken);
            
            if (accountDto == null)
            {
                throw new RequestFailedException($"Account with code {code} not found.");
            }

            InvalidateAccountCache(code);
            InvalidateAccountTransactionsCache(code);
            InvalidatePersonAccountsCache(accountDto.Person_Code);

            await _accountRepository.DeleteAccountAsync(code, cancellationToken);
        }

        public async Task<AccountResponse> UpsertAccountAsync(AccountRequest account, CancellationToken cancellationToken)
        {
            AccountDto accountDto = new()
            {
                Code = account.Code,
                Person_Code = account.PersonCode,
                Account_Number = account.AccountNumber,
                Outstanding_Balance = account.OutstandingBalance,
                Account_Status_Id = account.AccountStatusId
            };

            var returnCode = await _accountRepository.UpsertAccountAsync(accountDto, cancellationToken);
            var newAccount = await _accountRepository.GetAccountAsync(returnCode, cancellationToken);

            InvalidateAccountCache(returnCode);
            InvalidateAccountTransactionsCache(returnCode);
            InvalidatePersonAccountsCache(newAccount!.Person_Code);

            return await GetAccountAsync(newAccount.Code, cancellationToken);
        }

        public async Task<List<AccountStatusResponse>> GetAccountStatusesAsync(CancellationToken cancellationToken)
        {
            string cacheKey = $"{AccountStatusCacheKey}";

            IEnumerable<AccountStatusDto>? accountStatusDtos;

            if (_cache.TryGetValue(cacheKey, out List<AccountStatusDto>? cachedAccountStatuses))
            {
                accountStatusDtos = cachedAccountStatuses;
            }
            else
            {
                accountStatusDtos = await _accountStatusRepository.GetAccountStatusesAsync(cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, accountStatusDtos, cacheEntryOptions);
            }

            if (accountStatusDtos == null || !accountStatusDtos.Any())
            {
                return [];
            }

            var accountStatuses = accountStatusDtos
                .Select(a => new AccountStatusResponse
                {
                    Id = a.Id,
                    Description = a.Description!
                })
                .ToList();

            return accountStatuses;
        }

        private void InvalidateAccountCache(int accountCode)
        {
            _cache.Remove($"{AccountCacheKey}_Code_{accountCode}");
        }

        private void InvalidateAccountTransactionsCache(int accountCode)
        {
            _cache.Remove($"{AccountTransactionsCacheKey}_Code_{accountCode}");
        }

        private void InvalidatePersonAccountsCache(int personCode)
        {
            _cache.Remove($"{PersonAccountsCacheKey}_Code_{personCode}");
        }

        private async Task<AccountDto> GetAccountDtoAsync(string accountCacheKey, int accountCode, CancellationToken cancellationToken)
        {
            AccountDto? accountDto;

            if (_cache.TryGetValue(accountCacheKey, out AccountDto? cachedAccount))
            {
                accountDto = cachedAccount ?? default;
            }
            else
            {
                accountDto = await _accountRepository.GetAccountAsync(accountCode, cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(accountCacheKey, accountDto, cacheEntryOptions);
            }

            if (accountDto == null)
            {
                throw new RequestFailedException($"Account with code {accountCode} not found.");
            }

            return accountDto;
        }

        private async Task<IEnumerable<TransactionDto>> GetAccountTransactionDtos(string accountTransactionsCacheKey, int accountCode, CancellationToken cancellationToken)
        {
            IEnumerable<TransactionDto> transactionDtos;

            if (_cache.TryGetValue(accountTransactionsCacheKey, out List<TransactionDto>? cachedTransactions))
            {
                transactionDtos = cachedTransactions ?? [];
            }
            else
            {
                transactionDtos = await _transactionRepository.GetTransactionsByAccountCodeAsync(accountCode, cancellationToken);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _cache.Set(accountTransactionsCacheKey, transactionDtos, cacheEntryOptions);
            }

            return transactionDtos;
        }

        private async Task<IEnumerable<AccountDto>> GetPersonAccountDtos(string cacheKey, int personCode, CancellationToken cancellationToken)
        {
            IEnumerable<AccountDto>? accountDtos;

            if (_cache.TryGetValue(cacheKey, out List<AccountDto>? cachedAccounts))
            {
                accountDtos = cachedAccounts ?? [];
            }
            else
            {
                accountDtos = await _accountRepository.GetAccountsByPersonCodeAsync(personCode, cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, accountDtos, cacheEntryOptions);
            }

            return accountDtos;
        }
    }
}
