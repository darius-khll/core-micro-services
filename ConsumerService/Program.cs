using Autofac;
using ConsumerService.Implementations.Configurations;
using Microsoft.Extensions.Configuration;

namespace ConsumerService
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationRoot configuration = new EnvironmentConfigs().InitializedEnvironmentConfigs();

            ConsumerOptions consumerOptions = new GetConsumerOptions().GetOptions(configuration);

            ContainerBuilder builder = new DependenciesConfigs().InitializedDependencies(configuration, consumerOptions);

            new DependenciesConfigs().StartUpConfigurations(builder, consumerOptions);

            new DependenciesConfigs().RunConsumer(builder).GetAwaiter().GetResult();

        }
    }
}
