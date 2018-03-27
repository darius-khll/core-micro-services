using FirstService.Repository.Implementations;
using MassTransit;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsumerService
{
    public class PubSubConsumer : IConsumer<IPubSub>
    {
        public IMongoClient _mongoClient { get; }

        public PubSubConsumer(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            IMongoDatabase db = _mongoClient.GetDatabase("secondDb");

            await db.GetCollection<User>("Users").InsertOneAsync(new User { Name = "abc1", Age = 10 });

            List<User> users = (await db.GetCollection<User>("Users").FindAsync(c => c.Name == "abc1")).ToList();

            await Task.Delay(5000);
        }
    }
}
