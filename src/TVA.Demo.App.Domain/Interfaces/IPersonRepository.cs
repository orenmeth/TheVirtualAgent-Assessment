using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IPersonRepository
    {
        Task<PersonDto?> GetPersonAsync(int code, CancellationToken cancellationToken);
        Task<IEnumerable<PersonDto>> GetPersonsAsync(CancellationToken cancellationToken);
        Task UpsertPersonAsync(PersonDto person, CancellationToken cancellationToken);
        Task DeletePersonAsync(int code, bool deleteRelatedAccounts, CancellationToken cancellationToken);
    }
}