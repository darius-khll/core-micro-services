using Common.Implementations;
using System.Threading.Tasks;

namespace FirstService.Repository
{
    public interface IFirstBusiness
    {
        Task<string> UserHandlerAsync(string user, string name);
        int Sum(int a, int b);
        Task<string> UserTest(string user, string name);
    }

    public class FirstBusiness : IFirstBusiness
    {
        private readonly IHttpService _http;
        private readonly IRedisRepository _redisRepository;

        public FirstBusiness(IHttpService http, IRedisRepository redisRepository)
        {
            _http = http;
            _redisRepository = redisRepository;
        }

        public async Task<string> UserHandlerAsync(string user, string name)
        {
            await _redisRepository.SetUser(user, name);
            string foo = await _redisRepository.GetUser(user);

            string content = await _http.GetStringAsync("http://secondService");

            return string.Format($"First Service requested: {content} - {foo}");
        }

        public async Task<string> UserTest(string user, string name)
        {
            await _redisRepository.SetUser(user, name);
            string foo = await _redisRepository.GetUser(user);
            return foo;
        }

        public int Sum(int a, int b)
        {
            return a + b;
        }
    }
}
