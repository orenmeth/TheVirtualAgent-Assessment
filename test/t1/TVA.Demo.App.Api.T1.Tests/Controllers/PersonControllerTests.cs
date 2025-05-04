using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using TVA.Demo.App.Api.Controllers;
using TVA.Demo.App.Application.Services;
using TVA.Demo.App.Domain.Entities;
using TVA.Demo.App.Domain.Interfaces;
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

        public struct Stubs
        {
            public IDapperWrapper DapperWrapper { get; set; }
        }

        private static PersonController GetComponentUnderTest(IDapperWrapper dapperWrapper)
        {
            var logger = Substitute.For<ILogger<PersonController>>();
            var connectionFactory = Substitute.For<IConnectionFactory>();
            var dbConnectionProvider = Substitute.For<IDbConnectionProvider>();
            var memoryCache = Substitute.For<IMemoryCache>();
            var personRepository = new PersonRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var accountRepository = new AccountRepository(connectionFactory, dbConnectionProvider, dapperWrapper);
            var personService = new PersonService(personRepository, accountRepository, memoryCache);
            return new PersonController(logger, personService);
        }

        [TestCase("Code", "null", 1, 10, false)]
        [TestCase("Code", "null", 1, 10, true)]
        [TestCase("Code", "null", 2, 10, false)]
        [TestCase("Code", "null", 2, 10, true)]
        public async Task GetPersonsAsync_GivenValidArguments_ShouldReturnOkWithPersons(string sortBy, string filter, int page, int pageSize, bool descending)
        {
            // Arrange
            var dapperWrapper = Substitute.For<IDapperWrapper>();
            dapperWrapper.QueryAsync<PersonDto>(Arg.Any<SqlConnection>(), Arg.Any<CommandDefinition>())
                .Returns(
                [
                    new PersonDto { Code = 1, Name = "John", Surname = "Doe", Id_Number = "123456789" },
                    new PersonDto { Code = 2, Name = "Jane", Surname = "Smith", Id_Number = "987654321" },
                    new PersonDto { Code = 3, Name = "Alice", Surname = "Johnson", Id_Number = "111222333" },
                    new PersonDto { Code = 4, Name = "Bob", Surname = "Brown", Id_Number = "444555666" },
                    new PersonDto { Code = 5, Name = "Charlie", Surname = "Davis", Id_Number = "777888999" },
                    new PersonDto { Code = 6, Name = "David", Surname = "Wilson", Id_Number = "000111222" },
                    new PersonDto { Code = 7, Name = "Eve", Surname = "Garcia", Id_Number = "333444555" },
                    new PersonDto { Code = 8, Name = "Frank", Surname = "Martinez", Id_Number = "666777888" },
                    new PersonDto { Code = 9, Name = "Grace", Surname = "Lopez", Id_Number = "999000111" },
                    new PersonDto { Code = 10, Name = "Hank", Surname = "Gonzalez", Id_Number = "222333444" },
                    new PersonDto { Code = 11, Name = "Ivy", Surname = "Hernandez", Id_Number = "555666777" },
                    new PersonDto { Code = 12, Name = "Jack", Surname = "Clark", Id_Number = "888999000" },
                    new PersonDto { Code = 13, Name = "Kathy", Surname = "Rodriguez", Id_Number = "111222333" },
                    new PersonDto { Code = 14, Name = "Leo", Surname = "Lewis", Id_Number = "444555666" },
                    new PersonDto { Code = 15, Name="Mia", Surname="Walker",Id_Number="777888999"},
                ]);

            var controller = GetComponentUnderTest(dapperWrapper);
            var cancellationToken = new CancellationToken();

            // Act
            var result = await controller.GetPersonsAsync(sortBy, filter, page, pageSize, descending, cancellationToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;

            okResult.Should().NotBeNull();
            okResult.Value.Should().BeOfType<PagedResponse<PersonResponse>>();

            var pagedResponse = (PagedResponse<PersonResponse>)okResult.Value;
            pagedResponse.CurrentPage.Should().Be(page);
            pagedResponse.TotalItems.Should().Be(15);
            pagedResponse.TotalPages.Should().Be(2);
            pagedResponse.PageSize.Should().Be(pageSize);
            pagedResponse.Items.Should().BeOfType<List<PersonResponse>>();

            if (descending)
            {
                pagedResponse.Items.Should().BeInDescendingOrder(p => p.Code);
            }
            else
            {
                pagedResponse.Items.Should().BeInAscendingOrder(p => p.Code);
            }
        }
    }
}
