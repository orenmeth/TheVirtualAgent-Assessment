using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TVA.Demo.App.Application.Interfaces;
using TVA.Demo.App.Application.Services;
using TVA.Demo.App.Domain.Interfaces;
using TVA.Demo.App.Infrastructure.Factories;
using TVA.Demo.App.Infrastructure.Providers;
using TVA.Demo.App.Infrastructure.Repositories;

namespace TVA.Demo.App.Api.T1.Tests.Extensions
{
    [ExcludeFromCodeCoverage(Justification = "Service lifetime registrations")]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddInfrastructureFactories();
            serviceCollection.AddInfrastructureProviders();

            serviceCollection.AddInfrastructureRepositories();
            return serviceCollection;
        }

        internal static IServiceCollection AddInfrastructureFactories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConnectionFactory, SqlConnectionFactory>();
            return serviceCollection;
        }

        internal static IServiceCollection AddInfrastructureProviders(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDbConnectionProvider, DbConnectionProvider>();
            return serviceCollection;
        }

        internal static IServiceCollection AddInfrastructureRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDapperWrapper, DapperWrapper>();
            serviceCollection.AddSingleton<IPersonRepository, PersonRepository>();
            serviceCollection.AddSingleton<IAccountRepository, AccountRepository>();
            serviceCollection.AddSingleton<ITransactionRepository, TransactionRepository>();
            return serviceCollection;
        }

        internal static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPersonService, PersonService>();
            serviceCollection.AddSingleton<IAccountService, AccountService>();
            serviceCollection.AddSingleton<ITransactionService, TransactionService>();
            return serviceCollection;
        }

        public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddApplicationServices();
            return serviceCollection;
        }
    }
}
