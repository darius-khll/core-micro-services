using StackExchange.Redis;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface IRedisRepository
    {
        Task<string> GetUser(string user);
        Task SetUser(string user, string name);
    }

    /*
     *  redis repository for it's database
     */
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase  _client;

        public RedisRepository(IDatabase client)
        {
            _client = client;
        }        

        public async Task<string> GetUser(string user)
        {
            string name = await _client.StringGetAsync(user);
            return name;
        }

        public async Task SetUser(string user, string name)
        {
            await _client.StringGetSetAsync(user, name);
        }
    }
}
