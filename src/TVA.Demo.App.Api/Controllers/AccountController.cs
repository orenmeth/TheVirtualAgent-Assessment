using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Models.Requests;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController(ILogger<AccountController> logger, IAccountService accountService) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IAccountService _accountService = accountService;

        [HttpGet("GetAccount/{code}")]
        public async Task<IActionResult> GetAccountByCodeAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _accountService.GetAccountAsync(code, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching account with code {Code}.", code);
                return BadRequest(code);
            }
        }

        [HttpGet("GetAccountStatuses")]
        public async Task<IActionResult> GetAccountStatusesAsync(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _accountService.GetAccountStatusesAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching account statuses.");
                return BadRequest();
            }
        }

        [HttpDelete("DeleteAccount/{code}")]
        public async Task<IActionResult> DeleteAccountAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                await _accountService.DeleteAccountAsync(code, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting account.");
                return BadRequest();
            }
        }

        [HttpPost("UpsertAccount")]
        public async Task<IActionResult> UpsertAccountAsync(AccountRequest account, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _accountService.UpsertAccountAsync(account, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting account.");
                return BadRequest(account);
            }
        }
    }
}