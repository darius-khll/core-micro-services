using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace FirstService.Repository
{
    public interface ICacheBusiness
    {
        Task<string> CacheIfNotExist();
        Task<string> RemoveCache();
    }


    public class CacheBusiness : ICacheBusiness
    {
        private readonly IDistributedCache _distributedCache;
        private const string _key = "TheTime";

        public CacheBusiness(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> CacheIfNotExist()
        {
            var existingTime = await _distributedCache.GetStringAsync(_key);
            if (!string.IsNullOrEmpty(existingTime))
            {
                return "Fetched from cache : " + existingTime;
            }
            else
            {
                existingTime = DateTime.UtcNow.ToString();
                await _distributedCache.SetStringAsync(_key, existingTime);
                return "Added to cache : " + existingTime;
            }
        }

        public async Task<string> RemoveCache()
        {
            await _distributedCache.SetStringAsync(_key, "");
            return "done";
        }
    }
}
