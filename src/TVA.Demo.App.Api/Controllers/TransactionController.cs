using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;

namespace TVA.Demo.App.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransactionController(ILogger<TransactionController> logger, ITransactionService transactionService) : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger = logger;
        private readonly ITransactionService _transactionService = transactionService;

        [Authorize]
        [HttpGet("GetTransaction/{code}")]
        public async Task<IActionResult> GetTransactionByCodeAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _transactionService.GetTransactionAsync(code, cancellationToken));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for GetTransaction: {Message}", ex.Message);
                return NotFound(new ErrorResponse<int> { Item = code, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching transaction with code {Code}.", code);
                return BadRequest(code);
            }
        }

        [Authorize]
        [HttpDelete("DeleteTransaction/{code}")]
        public async Task<IActionResult> DeleteTransactionAsync(int code, CancellationToken cancellationToken)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(code, cancellationToken);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for DeleteTransaction: {Message}", ex.Message);
                return NotFound(new ErrorResponse<int> { Item = code, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting transaction.");
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("UpsertTransaction")]
        public async Task<IActionResult> UpsertTransactionAsync(TransactionRequest transaction, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _transactionService.UpsertTransactionAsync(transaction, cancellationToken));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request for UpsertTransaction: {Message}", ex.Message);
                return NotFound(new ErrorResponse<TransactionRequest> { Item = transaction, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while upserting transaction.");
                return BadRequest(transaction);
            }
        }
    }
}
