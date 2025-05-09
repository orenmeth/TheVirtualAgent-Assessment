using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using NUnit.Framework;
using System.Data;
using TVA.Demo.App.Api.Controllers;
using TVA.Demo.App.Application.Services;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Domain.Models.Requests;
using TVA.Demo.App.Domain.Models.Responses;
using TVA.Demo.App.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using NSubstitute.Core;
using NUnit.Framework.Internal;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Drawing;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System;

namespace TVA.Demo.App.Api.T1.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTests
    {
        private static AccountController GetComponentUnderTest(
            IDapperWrapper dapperWrapper,
            ILogger<AccountController> logger,
            IMemoryCache? memoryCache = null)
        {
            var actualMemoryCache = memoryCache ?? Substitute.For<IMemoryCache>();
            object? outValue;
            actualMemoryCache.TryGetValue(Arg.Any<object>(), out outValue).Returns(false);

            var connectionFactory = Substitute.For<IConnectionFactory>();
            var dbConnectionProvider = Substitute.For<IDbConnectionProvider>();

            var accountRepository = new AccountRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var accountStatusRepository = new AccountStatusRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var transactionRepository = new TransactionRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var accountService = new AccountService(accountStatusRepository, accountRepository, transactionRepository, actualMemoryCache);
            return new AccountController(logger, accountService);
        }

        private AccountDto GetSampleAccountDto(int code = 1, int personCode = 101) =>
            new() { Code = code, Person_Code = personCode, Account_Number = $"ACC{code:D5}", Outstanding_Balance = 100.50m, Account_Status_Id = 1 };

        private List<TransactionDto> GetSampleTransactionDtos(int accountCode, int count = 2) =>
            Enumerable.Range(1, count)
                .Select(i => new TransactionDto { Code = (accountCode * 100) + i, Account_Code = accountCode, Capture_Date = DateTime.UtcNow.AddDays(-i), Transaction_Date = DateTime.UtcNow.AddDays(-i), Amount = i * 10m, Description = $"Transaction {i} for Acc {accountCode}" })
                .ToList();

        private List<AccountStatusDto> GetSampleAccountStatusDtos() =>
        [
            new AccountStatusDto { Id = 1, Description = "Active" },
            new AccountStatusDto { Id = 2, Description = "Inactive" },
            new AccountStatusDto { Id = 3, Description = "Closed" }
        ];

        [Test]
        public async Task GetAccountByCodeAsync_WhenAccountExists_ShouldReturnOkWithAccountResponse()
        {
            // Arrange
            var accountCode = 1;
            var personCode = 101;
            var accountDto = GetSampleAccountDto(accountCode, personCode);
            var transactionDtos = GetSampleTransactionDtos(accountCode);

            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<AccountController>>();
            var memoryCache = Substitute.For<IMemoryCache>();

            object? accCacheVal, tranCacheVal;
            memoryCache.TryGetValue(Arg.Is($"AccountData_Code_{accountCode}"), out accCacheVal).Returns(false);
            memoryCache.TryGetValue(Arg.Is($"AccountTransactionsData_Code_{accountCode}"), out tranCacheVal).Returns(false);

            dapperWrapper.QuerySingleOrDefaultAsync<AccountDto?>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .Returns(Task.FromResult<AccountDto?>(accountDto));

            dapperWrapper.QueryAsync<TransactionDto>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .Returns(Task.FromResult(transactionDtos.AsEnumerable()));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetAccountByCodeAsync(accountCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeOfType<AccountResponse>();
            var response = (AccountResponse)okResult.Value!;
            response.Code.Should().Be(accountCode);
            response.AccountNumber.Should().Be(accountDto.Account_Number);
            response.Transactions.Should().HaveCount(transactionDtos.Count);
        }

        [Test]
        public async Task GetAccountByCodeAsync_WhenAccountNotFound_ShouldReturnNotFoundWithMessage()
        {
            // Arrange
            var accountCode = 999;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<AccountController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? accCacheVal;
            memoryCache.TryGetValue(Arg.Is($"AccountData_Code_{accountCode}"), out accCacheVal).Returns(false);

            dapperWrapper.QuerySingleOrDefaultAsync<AccountDto?>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .Returns(Task.FromResult<AccountDto?>(null));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetAccountByCodeAsync(accountCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeOfType<ErrorResponse<int>>();
        }

        [Test]
        public async Task GetAccountByCodeAsync_WhenDapperThrowsSqlException_ShouldReturnStatus500WithErrorResponse()
        {
            // Arrange
            var accountCode = 123;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<AccountController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? accCacheVal;
            memoryCache.TryGetValue(Arg.Is($"AccountData_Code_{accountCode}"), out accCacheVal).Returns(false);

            var sqlException = new Exception("DB connection failed");
            dapperWrapper.QuerySingleOrDefaultAsync<AccountDto?>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .ThrowsAsync(sqlException);

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetAccountByCodeAsync(accountCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeOfType<ErrorResponse<int>>();
            var errorResponse = (ErrorResponse<int>)objectResult.Value!;
            errorResponse.Item.Should().Be(accountCode);
            errorResponse.ErrorMessage.Should().Be("An unexpected error occurred while account.");
        }

        [Test]
        public async Task GetAccountStatusesAsync_WhenStatusesExist_ShouldReturnOkWithStatusList()
        {
            // Arrange
            var statusDtos = GetSampleAccountStatusDtos();
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<AccountController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? cacheVal;
            memoryCache.TryGetValue(Arg.Is("AccountStatusData"), out cacheVal).Returns(false);


            dapperWrapper.QueryAsync<AccountStatusDto>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .Returns(Task.FromResult(statusDtos.AsEnumerable()));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetAccountStatusesAsync(CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeOfType<List<AccountStatusResponse>>();
            var response = (List<AccountStatusResponse>)okResult.Value!;
            response.Should().HaveCount(statusDtos.Count);
            response.First().Description.Should().Be(statusDtos.First().Description);
        }

        [Test]
        public async Task GetAccountStatusesAsync_WhenDapperThrowsSqlException_ShouldReturnStatus500WithErrorResponse()
        {
            // Arrange
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<AccountController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? cacheVal;
            memoryCache.TryGetValue(Arg.Is("AccountStatusData"), out cacheVal).Returns(false);

            var sqlException = new Exception("DB error fetching statuses");
            dapperWrapper.QueryAsync<AccountStatusDto>(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetAccountStatuses"))
                .ThrowsAsync(sqlException);

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.GetAccountStatusesAsync(CancellationToken.None);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeOfType<ErrorResponse<GetPersonsRequest>>();
            var errorResponse = (ErrorResponse<GetPersonsRequest>)objectResult.Value!;
            errorResponse.ErrorMessage.Should().Be("An unexpected error occurred while fetching account statuses.");
            logger.Received(1).LogError(sqlException, "An unexpected error occurred while fetching account statuses.");
        }

        [Test]
        public async Task DeleteAccountAsync_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var accountCode = 1;
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<AccountController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? accCacheVal, transCacheVal, personAccCacheVal;
            memoryCache.TryGetValue(Arg.Any<string>(), out accCacheVal).Returns(false);
            memoryCache.TryGetValue(Arg.Is($"AccountData_Code_{accountCode}"), out accCacheVal).Returns(false);
            memoryCache.TryGetValue(Arg.Is($"AccountTransactionsData_Code_{accountCode}"), out transCacheVal).Returns(false);

            var accountDto = GetSampleAccountDto(accountCode, personCode: 777);
            dapperWrapper.QuerySingleOrDefaultAsync<AccountDto?>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
               .Returns(Task.FromResult<AccountDto?>(accountDto));

            dapperWrapper.ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .Returns(Task.FromResult(1));

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.DeleteAccountAsync(accountCode, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkResult>();
            await dapperWrapper.Received(1).ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>());
        }

        //[Test]
        //public async Task UpsertAccountAsync_WhenUpdatingExistingAccount_ShouldReturnOkWithUpdatedAccount()
        //{
        //    // Arrange
        //    var accountCodeToUpdate = 12;
        //    var request = new AccountRequest
        //    {
        //        Code = accountCodeToUpdate,
        //        PersonCode = 101,
        //        AccountNumber = "ACC-UPDATED-001",
        //        OutstandingBalance = 1250.75m,
        //        AccountStatusId = 1
        //    };

        //    var dtoAfterUpdate = new AccountDto
        //    {
        //        Code = accountCodeToUpdate,
        //        Person_Code = request.PersonCode,
        //        Account_Number = request.AccountNumber,
        //        Outstanding_Balance = request.OutstandingBalance,
        //        Account_Status_Id = request.AccountStatusId
        //    };
        //    var relatedTransactionDtos = GetSampleTransactionDtos(accountCodeToUpdate);

        //    var dapperWrapper = Substitute.For<IDapperWrapper>();
        //    var logger = Substitute.For<ILogger<AccountController>>();
        //    var memoryCache = Substitute.For<IMemoryCache>();
        //    object? cacheVal;
        //    memoryCache.TryGetValue(Arg.Any<object>(), out cacheVal).Returns(false);

        //    dapperWrapper.ExecuteAsync(Arg.Any<SqlConnection>(),
        //        Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "UpsertAccount"))
        //        .Returns(Task.Run(() =>
        //        {
        //            // TODO: Fix this output parameter
        //            var parameters = new DynamicParameters();
        //            parameters.Add("@RETURN_CODE", 0, DbType.Int32, direction: ParameterDirection.Output);
        //            return parameters.Get<int>("@RETURN_CODE");
        //        }));

        //    dapperWrapper.QuerySingleOrDefaultAsync<AccountDto?>(Arg.Any<SqlConnection>(),
        //        Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetAccount"))
        //        .Returns(Task.FromResult<AccountDto?>(dtoAfterUpdate));

        //    dapperWrapper.QueryAsync<TransactionDto>(Arg.Any<SqlConnection>(),
        //        Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetTransactionsByAccountCode"))
        //        .Returns(Task.FromResult(relatedTransactionDtos.AsEnumerable()));

        //    var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

        //    // Act
        //    var result = await controller.UpsertAccountAsync(request, CancellationToken.None);

        //    // Assert
        //    result.Should().BeOfType<ObjectResult>();
        //    var okResult = (ObjectResult)result;
        //    okResult.Value.Should().BeOfType<AccountResponse>();
        //    var response = (AccountResponse)okResult.Value!;

        //    response.Code.Should().Be(accountCodeToUpdate);
        //    response.AccountNumber.Should().Be(request.AccountNumber);
        //    response.OutstandingBalance.Should().Be(request.OutstandingBalance);
        //    response.Transactions.Should().HaveCount(relatedTransactionDtos.Count);

        //    // Verify Dapper interactions
        //    await dapperWrapper.Received(1).ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "UpsertAccount"));
        //    await dapperWrapper.Received(1).QuerySingleOrDefaultAsync<AccountDto?>(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetAccount"));
        //    await dapperWrapper.Received(1).QueryAsync<TransactionDto>(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "GetTransactionsByAccountCode"));
        //}

        [Test]
        public async Task UpsertAccountAsync_WhenDapperThrowsSqlExceptionOnUpsert_ShouldReturnStatus500WithErrorResponse()
        {
            // Arrange
            var request = new AccountRequest { PersonCode = 1, AccountNumber = "ACC00123", OutstandingBalance = 100m, AccountStatusId = 1 };
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<AccountController>>();
            var memoryCache = Substitute.For<IMemoryCache>();

            var sqlException = new Exception("DB error on upsert");
            dapperWrapper.ExecuteAsync(Arg.Any<SqlConnection>(), Arg.Is<CommandDefinition>(cmd => cmd.CommandText == "UpsertAccount"))
                .ThrowsAsync(sqlException);

            var controller = GetComponentUnderTest(dapperWrapper, logger, memoryCache);

            // Act
            var result = await controller.UpsertAccountAsync(request, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().BeOfType<ErrorResponse<AccountRequest>>();
            var errorResponse = (ErrorResponse<AccountRequest>)objectResult.Value!;
            errorResponse.Item.Should().Be(request);
            errorResponse.ErrorMessage.Should().Be("An unexpected error occurred while upserting account.");
            logger.Received(1).LogError(sqlException, "Error occurred while upserting account.");
        }
    }
}
