using Autofac;
using ConsumerService.Implementations.Configurations;
using Microsoft.Extensions.Configuration;

namespace ConsumerService
{
    class Program
    {
        static void Main(string[] args)
        {
            var env = new EnvironmentConfigs();

            IConfigurationRoot configuration = env.InitializedEnvironmentConfigs();

            ConsumerOptions consumerOptions = env.GetOptions(configuration);

            ContainerBuilder builder = new DependenciesConfigs().InitializedDependencies(configuration, consumerOptions);

            new DependenciesConfigs().StartUpConfigurations(builder, consumerOptions);

            new DependenciesConfigs().RunConsumer(builder).GetAwaiter().GetResult();

        }
    }
}
