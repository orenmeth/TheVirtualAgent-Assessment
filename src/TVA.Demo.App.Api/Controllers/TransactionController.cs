using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Models.Requests;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService) : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger = logger;
        private readonly ITransactionService _transactionService = transactionService;

        [HttpGet("GetTransaction/{code}")]
        public async Task<IActionResult> GetTransactionByCodeAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _transactionService.GetTransactionAsync(code, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching transaction with code {Code}.", code);
                return BadRequest(code);
            }
        }

        [HttpDelete("DeleteTransaction/{code}")]
        public async Task<IActionResult> DeleteTransactionAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(code, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting transaction.");
                return BadRequest();
            }
        }

        [HttpPost("UpsertTransaction")]
        public async Task<IActionResult> UpsertTransactionAsync(TransactionRequest transaction, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _transactionService.UpsertTransactionAsync(transaction, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting transaction.");
                return BadRequest(transaction);
            }
        }
    }
}
