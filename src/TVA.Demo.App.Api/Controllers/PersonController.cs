using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Models;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PersonController(ILogger<PersonController> logger, IPersonService personService) : ControllerBase
    {
        private readonly ILogger<PersonController> _logger = logger;
        private readonly IPersonService _personService = personService;

        [HttpGet("GetPersons/page/{page}/pageSize/{pageSize}")]
        public async Task<IActionResult> GetPersonsAsync(CancellationToken cancellationToken, [FromRoute] int page = 1, [FromRoute] int pageSize = 10)
        {
            try
            {

                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var queryblePersons = await _personService.GetPersonsAsync(cancellationToken);
                var totalItems = await Task.Run(() => queryblePersons.Count, cancellationToken);

                var personsOnPage = await Task.Run(() => queryblePersons
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(), cancellationToken);

                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                var response = new
                {
                    items = personsOnPage,
                    totalItems,
                    totalPages,
                    currentPage = page,
                    pageSize
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching persons.");
                return BadRequest(Enumerable.Empty<Person>());
            }
        }

        [HttpGet("GetPerson/{code}")]
        public async Task<IActionResult> GetPersonByCode(int code, CancellationToken cancellationToken)
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
    }
}
