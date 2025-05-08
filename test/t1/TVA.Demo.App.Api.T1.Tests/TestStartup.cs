using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TVA.Demo.App.Api.T1.Tests.Extensions;

namespace TVA.Demo.App.Api.T1.Tests
{
    public static class TestStartup
    {
        public static IConfiguration? Configuration { get; set; }

        public static IHost? Host { get; private set; }

        public static void Configure()
        {
            var builder = new HostBuilder()
                .ConfigureServices((services) =>
                {
                    SetupApplicationConfiguration();
                    
                    services.AddMemoryCache();

                    services.AddMvcCore().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

                    services.AddInfrastructure();
                    services.AddApplication();
                })
                .UseDefaultServiceProvider(options => options.ValidateScopes = true);
            Host = builder.Build();
        }

        public static T GetInstance<T>()
        {
            var serviceProvider = Host!.Services;
            var service = serviceProvider.GetService<T>()!;

            return service;
        }

        private static void SetupApplicationConfiguration()
        {
            Configuration = new ConfigurationBuilder().SetBasePath(TestContext.CurrentContext.TestDirectory)
              .AddJsonFile("appsettings.t1.json", optional: true)
              .AddEnvironmentVariables()
              .Build();
        }
    }
}
