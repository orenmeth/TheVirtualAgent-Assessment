using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface IPersonService
    {
        Task<List<PersonResponse>> GetPersonsAsync(CancellationToken cancellationToken);
        Task<PersonResponse> GetPersonAsync(int code, CancellationToken cancellationToken);
        Task DeletePersonAsync(int code, CancellationToken cancellationToken);
        Task<PersonResponse> UpsertPersonAsync(PersonRequest person, CancellationToken cancellationToken);
    }
}