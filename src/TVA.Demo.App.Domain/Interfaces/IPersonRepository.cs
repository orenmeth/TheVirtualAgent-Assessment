using TVA.Demo.App.Domain.Entities;

namespace TVA.Demo.App.Domain.Interfaces
{
    public interface IPersonRepository
    {
        Task DeletePersonAsync(int code, bool deleteRelatedAccounts, CancellationToken cancellationToken);
        Task<PersonDto?> GetPersonAsync(int code, CancellationToken cancellationToken);
        Task<IEnumerable<PersonDto>> GetPersonsAsync(CancellationToken cancellationToken);
        Task UpsertPersonAsync(int? code, string name, string surname, string idNumber, CancellationToken cancellationToken);
    }
}