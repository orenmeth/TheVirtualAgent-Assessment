using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController(ILogger<AccountController> logger, IAccountService accountService) : ControllerBase
    {
        private readonly ILogger<AccountController> _logger = logger;
        private readonly IAccountService _accountService = accountService;

        [HttpGet("GetAccount/{code}")]
        public async Task<IActionResult> GetAccountByCode(int code, CancellationToken cancellationToken)
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
    }
}
