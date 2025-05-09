using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Application.Interfaces
{
    public interface IPersonService
    {
        Task<PagedResponse<PersonResponse>> GetPersonsAsync(
                string? filter,
                string? sortBy,
                bool isDescending,
                int pageNumber,
                int itemsPerPage,
                CancellationToken cancellationToken);
        Task<PersonResponse> GetPersonAsync(int code, CancellationToken cancellationToken);
        Task DeletePersonAsync(int code, CancellationToken cancellationToken);
        Task<PersonResponse> UpsertPersonAsync(PersonRequest person, CancellationToken cancellationToken);
    }
}