using TVA.Demo.App.Domain.Models;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface IPersonService
    {
        Task<List<Person>> GetPersonsAsync(CancellationToken cancellationToken);
        Task<Person> GetPersonAsync(int code, CancellationToken cancellationToken);
        Task DeletePersonAsync(int code, CancellationToken cancellationToken);
        Task<Person> UpsertPersonAsync(Person person, CancellationToken cancellationToken);
    }
}