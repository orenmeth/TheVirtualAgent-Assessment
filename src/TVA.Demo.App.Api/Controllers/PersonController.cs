using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Models;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonController(ILogger<PersonController> logger, IPersonService personService) : ControllerBase
    {
        private readonly ILogger<PersonController> _logger = logger;
        private readonly IPersonService _personService = personService;

        [HttpGet("GetPersons/page/{page}/pageSize/{pageSize}/descending/{descending}/sortBy/{sortBy}/filter/{filter}")]
        public async Task<IActionResult> GetPersonsAsync(
            [FromRoute] string sortBy,
            [FromRoute] string filter, 
            [FromRoute] int page = 1,
            [FromRoute] int pageSize = 10,
            [FromRoute] bool descending = false,
            CancellationToken cancellationToken = default)
        {
            try
            {

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var queryablePersons = await _personService.GetPersonsAsync(cancellationToken);

                if (queryablePersons.Count > 0 && !string.IsNullOrEmpty(filter) && !string.Equals(filter, "null", StringComparison.OrdinalIgnoreCase))
                {
                    queryablePersons = [.. queryablePersons.Where(q =>
                    {
                        if (q.Name!.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            q.Surname!.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                            q.IdNumber!.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    })];
                }

                if (string.IsNullOrEmpty(sortBy) || string.Equals(sortBy, "null", StringComparison.OrdinalIgnoreCase))
                {
                    sortBy = nameof(PersonRequest.Code);
                }

                var orderedPersons = queryablePersons.OrderBy(p =>
                {
                    var propertyInfo = p.GetType().GetProperties().FirstOrDefault(q => string.Equals(q.Name, sortBy, StringComparison.OrdinalIgnoreCase));
                    return propertyInfo == null
                        ? throw new ArgumentException($"Property '{sortBy}' not found on type '{p.GetType().Name}'.")
                        : propertyInfo.GetValue(p);
                }).ToList();

                if (descending)
                {
                    orderedPersons.Reverse();
                }

                var totalItems = await Task.Run(() => queryablePersons.Count, cancellationToken);

                var personsOnPage = await Task.Run(() => orderedPersons
                    !.Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(), cancellationToken);

                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var response = new PagedResponse<PersonResponse>
                {
                    Items = personsOnPage,
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching persons.");
                return BadRequest(Enumerable.Empty<PersonResponse>());
            }
        }

        [HttpGet("GetPerson/{code}")]
        public async Task<IActionResult> GetPersonByCodeAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _personService.GetPersonAsync(code, cancellationToken));
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching person with code {Code}.", code);
                return BadRequest(code);
            }
        }

        [HttpGet("DeletePerson/{code}")]
        public async Task<IActionResult> DeletePersonAsync(int code, CancellationToken cancellationToken)
        {
            try {
                await _personService.DeletePersonAsync(code, cancellationToken);
                return Ok();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error occurred while upserting person.");
                return BadRequest();
            }
        }

        [HttpPost("UpsertPerson")]
        public async Task<IActionResult> UpsertPersonAsync (PersonRequest person, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _personService.UpsertPersonAsync(person, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting person.");
                var error = new ErrorResponse<PersonRequest>
                {
                    Item = person,
                    ErrorMessage = ex.Message
                };
                return BadRequest(error);
            }
        }
    }
}
