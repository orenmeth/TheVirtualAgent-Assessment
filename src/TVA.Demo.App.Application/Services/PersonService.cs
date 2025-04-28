using Azure;
using Microsoft.Extensions.Caching.Memory;
using TVA.Demo.App.Application.Extensions;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Domain.Models;

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

        public async Task<List<Person>> GetPersonsAsync(CancellationToken cancellationToken)
        {
            string cacheKey = $"{PersonsCacheKey}";

            IEnumerable<PersonDto>? personDtos;
            if (_cache.TryGetValue(cacheKey, out List<PersonDto>? cachedPersons))
            {
                personDtos = cachedPersons;   
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

            if (personDtos == null || !personDtos.Any())
            {
                return [];
            }

            var persons = personDtos
                .Select(p => new Person
                {
                    Code = p.Code,
                    Name = p.Name,
                    Surname = p.Surname,
                    IdNumber = p.IdNumber,
                    Accounts = []
                })
                .ToList();

            return persons;
        }

        public async Task<Person> GetPersonAsync(int code, CancellationToken cancellationToken)
        {
            string personCacheKey = $"{PersonCacheKey}_Code_{code}";
            string personAccountsCacheKey = $"{PersonAccountsCacheKey}_Code_{code}";

            PersonDto? personDto;
            if (_cache.TryGetValue(personCacheKey, out PersonDto? cachedPerson))
            {
                personDto = cachedPerson;
            }
            else
            {
                personDto = await _personRepository.GetPersonAsync(code, cancellationToken);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };

                _cache.Set(personCacheKey, personDto, cacheEntryOptions);
            }

            if (personDto == null)
            {
                throw new RequestFailedException($"Person with code {code} not found.");
            }

            IEnumerable<AccountDto>? accountDtos;
            if (_cache.TryGetValue(personAccountsCacheKey, out List<AccountDto>? cachedAccounts))
            {
                accountDtos = cachedAccounts;
            }
            else
            {
                accountDtos = await _accountRepository.GetAccountsByPersonCodeAsync(code, cancellationToken);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _cache.Set(personAccountsCacheKey, accountDtos, cacheEntryOptions);
            }

            var accounts = accountDtos!
                .Select(p => new Account
                {
                    Code = p.Code,
                    PersonCode = p.PersonCode,
                    AccountNumber = p.AccountNumber,
                    OutstandingBalance = p.OutstandingBalance
                })
                .ToList();

            Person person = new() { Code = personDto.Code, Name = personDto.Name, Surname = personDto.Surname, IdNumber = personDto.IdNumber, Accounts = accounts };

            return person;
        }

        private void InvalidatePersonsCache()
        {
            var keysToRemove = _cache.GetKeysStartingWith(PersonsCacheKey).ToList();
            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
            }
        }
    }
}