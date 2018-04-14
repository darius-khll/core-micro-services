using Common.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace ConsumerService.Configurations
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

        public ConsumerOptions GetOptions(IConfigurationRoot configuration)
        {
            string mongoHost = configuration[$"{MongoOptions.GetConfigName}:{nameof(MongoOptions.host)}"];
            string rabbitHost = configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.host)}"];
            string rabbitUser = configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.user)}"];
            string rabbitPassword = configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.password)}"];

            return new ConsumerOptions
            {
                MongoHost = mongoHost,
                RabbitHost = rabbitHost,
                RabbitUser = rabbitUser,
                RabbitPassword = rabbitPassword
            };
        }
    }
}
