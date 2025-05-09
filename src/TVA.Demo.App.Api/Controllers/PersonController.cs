using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;
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

        [Authorize]
        [HttpGet("GetPersons")]
        public async Task<IActionResult> GetPersonsAsync([FromQuery] GetPersonsRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var pagedResponse = await _personService.GetPersonsAsync(
                    request.Filter,
                    request.SortBy,
                    request.Descending,
                    request.Page,
                    request.PageSize,
                    cancellationToken);

                var response = new PagedResponse<PersonResponse>
                {
                    Items = pagedResponse.Items,
                    TotalItems = pagedResponse.TotalItems,
                    CurrentPage = pagedResponse.CurrentPage,
                    PageSize = pagedResponse.PageSize
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for GetPersons: {Message}", ex.Message);
                return BadRequest(new ErrorResponse<GetPersonsRequest> { Item = request, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching persons.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<GetPersonsRequest> { Item = request, ErrorMessage = "An unexpected error occurred while fetching persons." });
            }
        }

        [Authorize]
        [HttpGet("GetPerson/{code}")]
        public async Task<IActionResult> GetPersonByCodeAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _personService.GetPersonAsync(code, cancellationToken));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for GetPerson: {Message}", ex.Message);
                return NotFound(new ErrorResponse<int> { Item = code, ErrorMessage = ex.Message });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching person with code {Code}.", code);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<int> { Item = code, ErrorMessage = "An unexpected error occurred while fetching person." });
            }
        }

        [Authorize]
        [HttpDelete("DeletePerson/{code}")]
        public async Task<IActionResult> DeletePersonAsync(int code, CancellationToken cancellationToken)
        {
            try {
                await _personService.DeletePersonAsync(code, cancellationToken);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for DeletePerson: {Message}", ex.Message);
                return NotFound(new ErrorResponse<int> { Item = code, ErrorMessage = ex.Message });
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error occurred while deleting person with code {Code}.", code);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<int> { Item = code, ErrorMessage = "An unexpected error occurred while deleting person." });
            }
        }

        [Authorize]
        [HttpPost("UpsertPerson")]
        public async Task<IActionResult> UpsertPersonAsync (PersonRequest person, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _personService.UpsertPersonAsync(person, cancellationToken));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for UpsertPerson: {Message}", ex.Message);
                return BadRequest(new ErrorResponse<PersonRequest> { Item = person, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting person.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<PersonRequest> { Item = person, ErrorMessage = "An unexpected error occurred while upserting person."});
            }
        }
    }
}
