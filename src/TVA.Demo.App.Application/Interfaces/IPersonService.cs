using TVA.Demo.App.Domain.Models;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface IPersonService
    {
        Task<List<Person>> GetPersonsAsync(CancellationToken cancellationToken);
        Task<Person> GetPersonAsync(int code, CancellationToken cancellationToken);
    }
}