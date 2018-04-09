using Common.Repositories.Mongo;
using ConsumerService.Business.Models;
using MassTransit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsumerService.Consumers
{
    public class PubSubConsumer : IConsumer<IPubSub>
    {
        public IMongoRepository<User> _mongoRepository { get; set; }

        public PubSubConsumer(IMongoRepository<User> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public async Task Consume(ConsumeContext<IPubSub> context)
        {
            User savedUser = await _mongoRepository.SaveAsync(new User { Name = "ali", Age = 20, Addresses = new List<string> { "Tehran", "Shiraz" } });

            User takenUser = await _mongoRepository.GetByIdAsync(savedUser.Id);

            ICollection<User> users = await _mongoRepository.FindAllAsync(u => u.Name == "ali");

            await _mongoRepository.DeleteAsync(takenUser.Id);
        }
    }
}
