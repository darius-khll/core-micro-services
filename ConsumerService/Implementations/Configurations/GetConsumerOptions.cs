using Common.Options;
using Microsoft.Extensions.Configuration;

namespace ConsumerService.Implementations.Configurations
{
    public class GetConsumerOptions
    {
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
