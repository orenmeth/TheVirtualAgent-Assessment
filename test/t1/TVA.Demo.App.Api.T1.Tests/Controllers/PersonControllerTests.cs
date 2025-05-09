using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System.Linq.Expressions;
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
    public class PersonControllerTests
    {
        private IServiceProvider? _serviceProvider;

        [SetUp]
        public void Setup()
        {
            TestStartup.Configure();

            var services = new ServiceCollection();
            services.AddControllers();
            _serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public void TearDown()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private static PersonController GetComponentUnderTest(IDapperWrapper dapperWrapper, ILogger<PersonController> logger)
        {
            var connectionFactory = Substitute.For<IConnectionFactory>();
            var dbConnectionProvider = Substitute.For<IDbConnectionProvider>();
            var memoryCache = Substitute.For<IMemoryCache>();
            var personRepository = new PersonRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var accountRepository = new AccountRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var personService = new PersonService(personRepository, accountRepository, memoryCache);
            return new PersonController(logger, personService);
        }

        private readonly IEnumerable<PersonDto> personDtos = [
                    new PersonDto { Code = 1, Name = "John", Surname = "Doe", Id_Number = "123456789" },
                    new PersonDto { Code = 2, Name = "Jane", Surname = "Smith", Id_Number = "987654321" },
                    new PersonDto { Code = 3, Name = "Alice", Surname = "Johnson", Id_Number = "111222444" },
                    new PersonDto { Code = 4, Name = "Bob", Surname = "Brown", Id_Number = "444555666" },
                    new PersonDto { Code = 5, Name = "Charlie", Surname = "Davis", Id_Number = "777888999" },
                    new PersonDto { Code = 6, Name = "David", Surname = "Wilson", Id_Number = "000111222" },
                    new PersonDto { Code = 7, Name = "Eve", Surname = "Garcia", Id_Number = "333444555" },
                    new PersonDto { Code = 8, Name = "Frank", Surname = "Martinez", Id_Number = "666777888" },
                    new PersonDto { Code = 9, Name = "Grace", Surname = "Lopez", Id_Number = "999000111" },
                    new PersonDto { Code = 10, Name = "Hank", Surname = "Gonzalez", Id_Number = "222333555" },
                    new PersonDto { Code = 11, Name = "Ivy", Surname = "Hernandez", Id_Number = "555666777" },
                    new PersonDto { Code = 12, Name = "Jack", Surname = "Clark", Id_Number = "888999000" },
                    new PersonDto { Code = 13, Name = "Kathy", Surname = "Rodriguez", Id_Number = "111222333" },
                    new PersonDto { Code = 14, Name = "Leo", Surname = "Lewis", Id_Number = "444555777" },
                    new PersonDto { Code = 15, Name="Mia", Surname="Walker",Id_Number="777888666"},
                ];

        [TestCase("Code", "null", 1, 10, false, TestName = "GetPersonsAsync_Page1_Size10_AscByCode_NoFilter")]
        [TestCase("Code", "null", 1, 10, true, TestName = "GetPersonsAsync_Page1_Size10_DescByCode_NoFilter")]
        [TestCase("Code", "null", 2, 10, false, TestName = "GetPersonsAsync_Page2_Size10_AscByCode_NoFilter")]
        [TestCase("Code", "null", 2, 10, true, TestName = "GetPersonsAsync_Page2_Size10_DescByCode_NoFilter")]
        [TestCase("Code", "null", 1, 5, false, TestName = "GetPersonsAsync_P1_S5_AscByCode_NoFilter")]
        [TestCase("Code", "null", 1, 5, true, TestName = "GetPersonsAsync_P1_S5_DescByCode_NoFilter")]
        [TestCase("Code", "null", 2, 5, false, TestName = "GetPersonsAsync_P2_S5_AscByCode_NoFilter")]
        [TestCase("Code", "null", 2, 5, true, TestName = "GetPersonsAsync_P2_S5_DescByCode_NoFilter")]
        [TestCase("Code", "null", 3, 5, false, TestName = "GetPersonsAsync_P3_S5_AscByCode_NoFilter_LastPage")]
        [TestCase("Code", "null", 3, 5, true, TestName = "GetPersonsAsync_P3_S5_DescByCode_NoFilter_LastPage")]
        [TestCase("Code", "null", 1, 15, false, TestName = "GetPersonsAsync_P1_S15_AscByCode_NoFilter_AllItems")]
        [TestCase("Code", "null", 1, 15, true, TestName = "GetPersonsAsync_P1_S15_DescByCode_NoFilter_AllItems")]
        [TestCase("Code", "null", 1, 20, false, TestName = "GetPersonsAsync_P1_S20_AscByCode_NoFilter_SizeOverTotal")]
        [TestCase("Code", "null", 1, 20, true, TestName = "GetPersonsAsync_P1_S20_DescByCode_NoFilter_SizeOverTotal")]
        [TestCase("Code", "null", 1, 7, false, TestName = "GetPersonsAsync_P1_S7_AscByCode_NoFilter")]
        [TestCase("Code", "null", 2, 7, true, TestName = "GetPersonsAsync_P2_S7_DescByCode_NoFilter")]
        [TestCase("Code", "null", 3, 7, false, TestName = "GetPersonsAsync_P3_S7_AscByCode_NoFilter_LastPageOneItem")]
        [TestCase("Name", "null", 1, 10, false, TestName = "GetPersonsAsync_P1_S10_AscByName_NoFilter")]
        [TestCase("Name", "null", 1, 10, true, TestName = "GetPersonsAsync_P1_S10_DescByName_NoFilter")]
        [TestCase("Name", "null", 2, 10, false, TestName = "GetPersonsAsync_P2_S10_AscByName_NoFilter")]
        [TestCase("Name", "null", 2, 10, true, TestName = "GetPersonsAsync_P2_S10_DescByName_NoFilter")]
        [TestCase("Name", "null", 1, 5, false, TestName = "GetPersonsAsync_P1_S5_AscByName_NoFilter")]
        [TestCase("Name", "null", 3, 5, true, TestName = "GetPersonsAsync_P3_S5_DescByName_NoFilter_LastPage")]
        [TestCase("Surname", "null", 1, 10, false, TestName = "GetPersonsAsync_P1_S10_AscBySurname_NoFilter")]
        [TestCase("Surname", "null", 1, 10, true, TestName = "GetPersonsAsync_P1_S10_DescBySurname_NoFilter")]
        [TestCase("Surname", "null", 1, 7, false, TestName = "GetPersonsAsync_P1_S7_AscBySurname_NoFilter")]
        [TestCase("Surname", "null", 3, 7, true, TestName = "GetPersonsAsync_P3_S7_DescBySurname_NoFilter_LastPageOneItem")]
        [TestCase("Id_Number", "null", 1, 10, false, TestName = "GetPersonsAsync_P1_S10_AscByIdNumber_NoFilter")]
        [TestCase("Id_Number", "null", 1, 10, true, TestName = "GetPersonsAsync_P1_S10_DescByIdNumber_NoFilter")]
        public async Task GetPersonsAsync_GivenValidArguments_ShouldReturnOkWithPersons(string sortBy, string filter, int page, int pageSize, bool descending)
        {
            // Arrange
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            dapperWrapper.QueryAsync<PersonDto>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>()).Returns(personDtos);

            var logger = Substitute.For<ILogger<PersonController>>();

            var controller = GetComponentUnderTest(dapperWrapper, logger);
            var cancellationToken = new CancellationToken();

            // Act
            var request = new GetPersonsRequest
            {
                SortBy = sortBy,
                Filter = filter,
                Page = page,
                PageSize = pageSize,
                Descending = descending
            };
            var result = await controller.GetPersonsAsync(request, cancellationToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult.Value.Should().BeOfType<PagedResponse<PersonResponse>>();

            var pagedResponse = (PagedResponse<PersonResponse>)okResult.Value;
            pagedResponse.CurrentPage.Should().Be(page);

            var totalItems = personDtos.Count();

            if (filter == "null")
            {
                pagedResponse.TotalItems.Should().Be(totalItems);
            }

            decimal totalPages = (decimal)totalItems / pageSize;
            pagedResponse.TotalPages.Should().Be((int)Math.Ceiling(totalPages));
            pagedResponse.PageSize.Should().Be(pageSize);
            pagedResponse.Items.Should().BeOfType<List<PersonResponse>>();

            Expression<Func<PersonResponse, object>>? keySelectorExpression = null;
            switch (sortBy.ToUpperInvariant())
            {
                case "CODE":
                    keySelectorExpression = p => p.Code;
                    break;
                case "NAME":
                    keySelectorExpression = p => p.Name;
                    break;
                case "SURNAME":
                    keySelectorExpression = p => p.Surname;
                    break;
                case "ID_NUMBER":
                    keySelectorExpression = p => p.IdNumber;
                    break;
                default:
                    throw new ArgumentException($"Invalid sortBy value: {sortBy}");
            }

            if (descending)
            {
                pagedResponse.Items.Should().BeInDescendingOrder(keySelectorExpression);
            }
            else
            {
                pagedResponse.Items.Should().BeInAscendingOrder(keySelectorExpression);
            }
        }

        [Test]
        public async Task GetPersonsAsync_WhenDapperThrowsSqlException_ShouldReturnErrorResponseAndLogEror()
        {
            // Arrange
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            var logger = Substitute.For<ILogger<PersonController>>();
            var memoryCache = Substitute.For<IMemoryCache>();
            object? cacheValue;
            memoryCache.TryGetValue(Arg.Any<object>(), out cacheValue).Returns(false);


            var expectedException = new Exception("Test exception");
            dapperWrapper.QueryAsync<PersonDto>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .ThrowsAsync(expectedException);

            var controller = GetComponentUnderTest(dapperWrapper, logger);
            var request = new GetPersonsRequest { Page = 1, PageSize = 10, SortBy = "Code" };
            var cancellationToken = new CancellationToken();

            // Act
            var result = await controller.GetPersonsAsync(request, cancellationToken);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeAssignableTo<ErrorResponse<GetPersonsRequest>>();
            logger.Received(1).LogError(expectedException, "Error occurred while fetching persons.");
        }
    }
}
