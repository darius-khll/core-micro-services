using Common.Repositories.Mongo;
using ConsumerService.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsumerService.Business
{
    public interface IMongoBusiness
    {
        Task HandleLogic();
    }

    public class MongoBusiness : IMongoBusiness
    {
        public readonly IMongoRepository<User> _mongoRepository;

        public MongoBusiness(IMongoRepository<User> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public async Task HandleLogic()
        {
            User savedUser = await _mongoRepository.SaveAsync(new User { Name = "ali", Age = 20, Addresses = new List<string> { "Tehran", "Shiraz" } });

            User takenUser = await _mongoRepository.GetByIdAsync(savedUser.Id);

            ICollection<User> users = await _mongoRepository.FindAllAsync(u => u.Name == "ali");

            await _mongoRepository.DeleteAsync(takenUser.Id);
        }
    }
}
