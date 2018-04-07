using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ConsumerService.Implementations.Configurations
{
    public class EnvironmentConfigs
    {
        public IConfigurationRoot InitializedEnvironmentConfigs()
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine(environmentName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine(configuration["redis:host"]);

            return configuration;
        }
    }
}
