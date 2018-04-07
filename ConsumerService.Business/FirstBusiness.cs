using Common.Repositories;
using System.Threading.Tasks;

namespace ConsumerService.Business
{
    public interface IFirstBusiness
    {
        Task<string> UserHandlerAsync(string user, string name);
        int Sum(int a, int b);
    }


    public class FirstBusiness : IFirstBusiness
    {
        private readonly IRedisRepository _redisRepository;

        public FirstBusiness(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        public async Task<string> UserHandlerAsync(string user, string name)
        {
            await _redisRepository.SetUser(user, name);
            string foo = await _redisRepository.GetUser(user);

            return string.Format($"{foo}");
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }
    }
}
