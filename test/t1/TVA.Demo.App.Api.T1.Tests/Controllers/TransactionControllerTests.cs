using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using TVA.Demo.App.Api.Controllers;
using TVA.Demo.App.Application.Services;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;
using TVA.Demo.App.Infrastructure.Repositories;

namespace TVA.Demo.App.Api.T1.Tests.Controllers
{
    [TestFixture]
    public class TransactionControllerTests
    {
        private TransactionController GetComponentUnderTest(
            IDapperWrapper dapperWrapper,
            ILogger<TransactionController> logger,
            IMemoryCache? memoryCache = null)
        {
            var actualMemoryCache = memoryCache ?? Substitute.For<IMemoryCache>();
            object? outValue;
            actualMemoryCache.TryGetValue(Arg.Any<object>(), out outValue).Returns(false);

            var connectionFactory = Substitute.For<IConnectionFactory>();
            var dbConnectionProvider = Substitute.For<IDbConnectionProvider>();

            var transactionRepository = new TransactionRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var transactionService = new TransactionService(transactionRepository, actualMemoryCache);

            return new TransactionController(logger, transactionService);
        }

        // --- Sample Data ---
        private TransactionDto GetSampleTransactionDto(int code = 1, int accountCode = 101, decimal amount = 50.00m) =>
            new() { Code = code, Account_Code = accountCode, Amount = amount, Description = $"Sample Transaction {code}", Transaction_Date = DateTime.UtcNow.AddDays(-1), Capture_Date = DateTime.UtcNow };

        private TransactionRequest GetSampleTransactionRequest(int accountCode = 101, decimal amount = 75.00m) =>
            new() { AccountCode = accountCode, Amount = amount, Description = "New Request Transaction", TransactionDate = DateTime.UtcNow.AddHours(-1).ToString() };

        // --- Tests for GetTransactionByCodeAsync ---

        [Test]
        public async Task GetTransactionByCodeAsync_WhenTransactionExists_ShouldReturnOkWithTransactionResponse()
        {
            // Arrange
            var transactionCode = 1;
            var transactionDto = GetSampleTransactionDto(transactionCode);
            var expectedResponse = new TransactionResponse { Code = transactionDto.Code, AccountCode = transactionDto.Account_Code, Amount = transactionDto.Amount, Description = transactionDto.Description, TransactionDate = transactionDto.Transaction_Date, CaptureDate = transactionDto.Capture_Date };

            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? cacheVal;
            memoryCache.TryGetValue(Arg.Is($"TransactionData_Code_{transactionCode}"), out cacheVal).Returns(false);

            dapperWrapper.QuerySingleOrDefaultAsync<TransactionDto?>(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetTransaction"))
                .Returns(Task.FromResult<TransactionDto?>(transactionDto));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetTransactionByCodeAsync(transactionCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public async Task GetTransactionByCodeAsync_WhenTransactionNotFound_ServiceReturnsNull_ShouldReturnBadRequestWithNullContent()
        {
            // Arrange
            var transactionCode = 999;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? cacheVal;
            memoryCache.TryGetValue(Arg.Is($"TransactionData_Code_{transactionCode}"), out cacheVal).Returns(false);

            dapperWrapper.QuerySingleOrDefaultAsync<TransactionDto?>(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetTransaction"))
                .Returns(Task.FromResult<TransactionDto?>(null));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetTransactionByCodeAsync(transactionCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var okResult = (BadRequestObjectResult)result;
            okResult.Value.Should().Be(transactionCode);
        }

        [Test]
        public async Task GetTransactionByCodeAsync_WhenServiceThrowsNotFoundException_ShouldReturnBadRequestWithCode()
        {
            // Arrange
            var transactionCode = 999;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? cacheVal;
            memoryCache.TryGetValue(Arg.Is($"TransactionData_Code_{transactionCode}"), out cacheVal).Returns(false);

            dapperWrapper.QuerySingleOrDefaultAsync<TransactionDto?>(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetTransaction"))
                .Returns(Task.FromResult<TransactionDto?>(null));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetTransactionByCodeAsync(transactionCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = (BadRequestObjectResult)result;
            badRequestResult.Value.Should().Be(transactionCode);
            logger.ReceivedWithAnyArgs(1).LogError(Arg.Any<Exception>(), "Error occurred while fetching transaction with code {Code}.", transactionCode);
        }


        [Test]
        public async Task GetTransactionByCodeAsync_WhenDapperThrowsSqlException_ShouldReturnBadRequestWithCode()
        {
            // Arrange
            var transactionCode = 123;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? cacheVal;
            memoryCache.TryGetValue(Arg.Is($"TransactionData_Code_{transactionCode}"), out cacheVal).Returns(false);

            var sqlException = new Exception("DB error on GetTransaction");
            dapperWrapper.QuerySingleOrDefaultAsync<TransactionDto?>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .ThrowsAsync(sqlException);

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetTransactionByCodeAsync(transactionCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = (BadRequestObjectResult)result;
            badRequestResult.Value.Should().Be(transactionCode);
        }

        // --- Tests for DeleteTransactionAsync ---

        [Test]
        public async Task DeleteTransactionAsync_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var transactionCode = 1;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();

            dapperWrapper.ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "DeleteTransaction"))
                .Returns(Task.FromResult(1));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.DeleteTransactionAsync(transactionCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkResult>();
            await dapperWrapper.Received(1).ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "DeleteTransaction"));
        }

        [Test]
        public async Task DeleteTransactionAsync_WhenDapperThrowsSqlException_ShouldReturnBadRequestAndLogEror()
        {
            // Arrange
            var transactionCode = 789;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();

            var sqlException = new Exception("DB error on DeleteTransaction");
            dapperWrapper.ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "DeleteTransaction"))
                .ThrowsAsync(sqlException);

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.DeleteTransactionAsync(transactionCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
            logger.Received(1).LogError(sqlException, "Error occurred while upserting transaction.");
        }

        // --- Tests for UpsertTransactionAsync ---

        [Test]
        public async Task UpsertTransactionAsync_WhenSuccessful_ShouldReturnOkWithUpsertedTransaction()
        {
            // Arrange
            var request = GetSampleTransactionRequest();
            var upsertedTransactionCode = 99;
            var transactionDtoAfterUpsert = new TransactionDto { Code = upsertedTransactionCode, Account_Code = request.AccountCode, Amount = request.Amount, Description = request.Description, Transaction_Date = DateTime.Parse(request.TransactionDate), Capture_Date = DateTime.UtcNow };
            var expectedResponse = new TransactionResponse { Code = upsertedTransactionCode, AccountCode = request.AccountCode, Amount = request.Amount, Description = request.Description, TransactionDate = DateTime.Parse(request.TransactionDate), CaptureDate = transactionDtoAfterUpsert.Capture_Date };


            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();

            dapperWrapper.ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "UpsertTransaction"))
                .Returns(Task.FromResult(1));

            dapperWrapper.QuerySingleOrDefaultAsync<TransactionDto?>(Arg.Any<SqlConnection>(),
                Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetTransaction"))
                .Returns(Task.FromResult<TransactionDto?>(transactionDtoAfterUpsert));

            object? cacheVal;
            memoryCache.TryGetValue(Arg.Is($"TransactionData_Code_{upsertedTransactionCode}"), out cacheVal).Returns(false);


            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.UpsertTransactionAsync(request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(r => r.CaptureDate));
            ((TransactionResponse)okResult.Value!).CaptureDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Test]
        public async Task UpsertTransactionAsync_WhenDapperThrowsSqlExceptionOnUpsert_ShouldReturnBadRequestWithRequestAndLogError()
        {
            // Arrange
            var request = GetSampleTransactionRequest();
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<TransactionController>>();
            var memoryCache = Substitute.For<IMemoryCache>();

            var sqlException = new Exception("DB error on UpsertTransaction");
            dapperWrapper.ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "UpsertTransaction"))
                .ThrowsAsync(sqlException);

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.UpsertTransactionAsync(request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = (BadRequestObjectResult)result;
            badRequestResult.Value.Should().Be(request);
            logger.Received(1).LogError(sqlException, "Error occurred while upserting transaction.");
        }
    }
}
