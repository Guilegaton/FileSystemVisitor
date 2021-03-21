using FileSystemVisitor.Common.Interfaces;
using FileSystemVisitor.Common.Models;
using FileSystemVisitor.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;

namespace FileSystemVisitor
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            ConfigureServices(configuration);

            var factory = _serviceProvider.GetRequiredService<IFileViewServiceFactory>();

            // Add lambda function for filtering, else keep null
            // var service = factory.CreateNewService(null);
            var service = factory.CreateNewService((entities) => entities.Where(file => file.Contains(".txt")).ToArray());

            // Add subscription for events with arguments
            //service.FilteredFileFinded += (ref FileViewerEventArgs args) =>
            //{
            //    args.StopSearching = true; ;
            //};
            var models = service.ViewFiles(@"#PATH#");
            foreach (var item in models)
            {
                Console.WriteLine(item);
            }
        }

        private static void ConfigureServices(IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();

            // Subscribe Services
            serviceCollection.AddScoped<IFileViewServiceFactory, FileViewServiceFactory>();
            serviceCollection.AddSingleton<ISystemEntityService, SystemEntityService>();

            serviceCollection.AddLogging(config =>
            {
                config.ClearProviders();
                config.AddNLog("nlog.config");
                config.SetMinimumLevel(LogLevel.Information);
            });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private static IConfiguration GetConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();

            return configuration;
        }
    }
}
