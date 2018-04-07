using FirstService.Repository.Implementations;
using MassTransit;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsumerService.Consumers
{
    public class PubSubConsumer : IConsumer<IPubSub>
    {
        public IMongoDatabase _mongoDb { get; }

        public PubSubConsumer(IMongoDatabase mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            await _mongoDb.GetCollection<User>("Users").InsertOneAsync(new User { Name = "abc1", Age = 10 });

            List<User> users = (await _mongoDb.GetCollection<User>("Users").FindAsync(c => c.Name == "abc1")).ToList();
        }
    }
}
