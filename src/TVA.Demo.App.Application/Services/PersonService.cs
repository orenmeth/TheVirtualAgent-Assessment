using Azure;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using TVA.Demo.App.Application.Extensions;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Domain.Models;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Application.Services
{
    public class PersonService(IPersonRepository personRepository, IAccountRepository accountRepository, IMemoryCache cache) : IPersonService
    {
        private readonly IPersonRepository _personRepository = personRepository;
        private readonly IAccountRepository _accountRepository = accountRepository;
        private readonly IMemoryCache _cache = cache;

        private const string PersonsCacheKey = "PersonsData";
        private const string PersonCacheKey = "PersonData";
        private const string PersonAccountsCacheKey = "PersonAccountsData";

        public async Task<PagedResponse<PersonResponse>> GetPersonsAsync(
                string? filter,
                string? sortBy,
                bool isDescending,
                int pageNumber,
                int itemsPerPage,
                CancellationToken cancellationToken)
        {
            string cacheKey = $"{PersonsCacheKey}";

            IEnumerable<PersonDto> personDtos = await GetPersonDtosAsync(cacheKey, cancellationToken);            

            var sortedAndFilteredPersonDtos = personDtos.Filter(filter).Sort(sortBy, isDescending);

            var totalItems = sortedAndFilteredPersonDtos.Count();

            var pagedDtos = sortedAndFilteredPersonDtos
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            var persons = pagedDtos
                .Select(p => new PersonResponse
                {
                    Code = p.Code,
                    Name = p.Name,
                    Surname = p.Surname,
                    IdNumber = p.Id_Number
                })
                .ToList();

            return new PagedResponse<PersonResponse>
            {
                Items = persons,
                TotalItems = totalItems,
                CurrentPage = pageNumber,
                PageSize = itemsPerPage
            };
        }

        public async Task<PersonResponse> GetPersonAsync(int code, CancellationToken cancellationToken)
        {
            string personCacheKey = $"{PersonCacheKey}_Code_{code}";
            string personAccountsCacheKey = $"{PersonAccountsCacheKey}_Code_{code}";

            PersonDto? personDto = await GetPersonDtoAsync(personCacheKey, code, cancellationToken);

            IEnumerable<AccountDto> accountDtos = await GetPersonAccountsAsync(personAccountsCacheKey, code, cancellationToken);

            var accounts = accountDtos
                .Select(a => new AccountResponse
                {
                    Code = a.Code,
                    PersonCode = a.Person_Code,
                    AccountNumber = a.Account_Number,
                    OutstandingBalance = a.Outstanding_Balance,
                    AccountStatusId = a.Account_Status_Id
                })
                .ToList();

            PersonResponse person = new() { Code = personDto.Code, Name = personDto.Name, Surname = personDto.Surname, IdNumber = personDto.Id_Number, Accounts = accounts };

            return person;
        }

        public async Task DeletePersonAsync(int code, CancellationToken cancellationToken)
        {
            string personCacheKey = $"{PersonCacheKey}_Code_{code}";
            string personAccountsCacheKey = $"{PersonAccountsCacheKey}_Code_{code}";

            var deleteRelatedAccounts = true;

            _cache.Remove(personCacheKey);
            _cache.Remove(personAccountsCacheKey);

            InvalidatePersonsCache();

            await _personRepository.DeletePersonAsync(code, deleteRelatedAccounts, cancellationToken);
        }

        public async Task<PersonResponse> UpsertPersonAsync(PersonRequest person, CancellationToken cancellationToken)
        {
            PersonDto personDto = new()
            {
                Code = person.Code,
                Name = person.Name,
                Surname = person.Surname,
                Id_Number = person.IdNumber,
            };

            var returnCode = await _personRepository.UpsertPersonAsync(personDto, cancellationToken);

            InvalidatePersonsCache();
            InvalidatePersonCache(returnCode);
            InvalidatePersonAccountsCache(returnCode);

            return await GetPersonAsync(returnCode, cancellationToken);
        }

        private void InvalidatePersonsCache()
        {
            _cache.Remove(PersonsCacheKey);
        }

        private void InvalidatePersonCache(int personCode)
        {
            _cache.Remove($"{PersonCacheKey}_Code_{personCode}");
        }

        private void InvalidatePersonAccountsCache(int personCode)
        {
            _cache.Remove($"{PersonAccountsCacheKey}_Code_{personCode}");
        }

        private async Task<IEnumerable<PersonDto>> GetPersonDtosAsync(string cacheKey, CancellationToken cancellationToken)
        {
            IEnumerable<PersonDto> personDtos;

            if (_cache.TryGetValue(cacheKey, out List<PersonDto>? cachedPersons))
            {
                personDtos = cachedPersons ?? [];
            }
            else
            {
                personDtos = await _personRepository.GetPersonsAsync(cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(cacheKey, personDtos, cacheEntryOptions);
            }

            return personDtos;
        }

        private async Task<PersonDto> GetPersonDtoAsync(string personCacheKey, int personCode, CancellationToken cancellationToken)
        {
            PersonDto? personDto;

            if (_cache.TryGetValue(personCacheKey, out PersonDto? cachedPerson))
            {
                personDto = cachedPerson ?? default;
            }
            else
            {
                personDto = await _personRepository.GetPersonAsync(personCode, cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(personCacheKey, personDto, cacheEntryOptions);
            }

            if (personDto == null)
            {
                throw new RequestFailedException($"Person with code {personCode} not found.");
            }

            return personDto;
        }

        private async Task<IEnumerable<AccountDto>> GetPersonAccountsAsync(string personAccountsCacheKey, int personCode, CancellationToken cancellationToken)
        {
            IEnumerable<AccountDto> accountDtos;

            if (_cache.TryGetValue(personAccountsCacheKey, out List<AccountDto>? cachedAccounts))
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
                _cache.Set(personAccountsCacheKey, accountDtos, cacheEntryOptions);
            }

            return accountDtos;
        }
    }
}