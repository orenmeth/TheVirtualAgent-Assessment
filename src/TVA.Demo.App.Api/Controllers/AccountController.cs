using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController(ILogger<AccountController> logger, IAccountService accountService) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IAccountService _accountService = accountService;

        [Authorize]
        [HttpGet("GetAccount/{code}")]
        public async Task<IActionResult> GetAccountByCodeAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                var account = await _accountService.GetAccountAsync(code, cancellationToken);
                if (account == null)
                {
                    _logger.LogInformation("Account with code {Code} not found.", code);
                    return NotFound($"Account with code {code} not found.");
                }
                return Ok(account);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for GetAccount: {Message}", ex.Message);
                return NotFound(new ErrorResponse<int> { Item = code, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching account with code {Code}.", code);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<int> { Item = code, ErrorMessage = "An unexpected error occurred while account." });
            }
        }

        [Authorize]
        [HttpGet("GetAccountStatuses")]
        public async Task<IActionResult> GetAccountStatusesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _accountService.GetAccountStatusesAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching account statuses.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<GetPersonsRequest> { ErrorMessage = "An unexpected error occurred while fetching account statuses." });
            }
        }

        [Authorize]
        [HttpDelete("DeleteAccount/{code}")]
        public async Task<IActionResult> DeleteAccountAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                await _accountService.DeleteAccountAsync(code, cancellationToken);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for DeleteAccount: {Message}", ex.Message);
                return NotFound(new ErrorResponse<int> { Item = code, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting account.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<int> { Item = code, ErrorMessage = "An unexpected error occurred while deleting account." });
            }
        }

        [Authorize]
        [HttpPost("UpsertAccount")]
        public async Task<IActionResult> UpsertAccountAsync(AccountRequest account, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _accountService.UpsertAccountAsync(account, cancellationToken));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for UpsertAccount: {Message}", ex.Message);
                return BadRequest(new ErrorResponse<AccountRequest> { Item = account, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting account.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse<AccountRequest> { Item = account, ErrorMessage = "An unexpected error occurred while upserting account." });
            }
        }
    }
}