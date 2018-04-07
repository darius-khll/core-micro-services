using Common.Repositories;
using System;
using System.Threading.Tasks;

namespace ConsumerService.Business
{
    public interface ICacheBusiness
    {
        Task<string> CacheIfNotExist();
        Task<string> RemoveCache();
    }


    public class CacheBusiness : ICacheBusiness
    {
        private readonly IRedisCachingRepository<string> _redisCaching;
        private const string _key = "TheTime";

        public CacheBusiness(IRedisCachingRepository<string> distributedCache)
        {
            _redisCaching = distributedCache;
        }

        /*
         *  caching using distributed cache
         */
        public async Task<string> CacheIfNotExist()
        {
            var existingTime = await _redisCaching.GetCachedData(_key);
            if (!string.IsNullOrEmpty(existingTime))
            {
                return "Fetched from cache : " + existingTime;
            }
            else
            {
                existingTime = DateTime.UtcNow.ToString();
                await _redisCaching.SetCachedData(_key, existingTime);
                return "Added to cache : " + existingTime;
            }
        }

        /*
         *  remove from distributed cache
         */
        public async Task<string> RemoveCache()
        {
            await _redisCaching.SetCachedData(_key, "");
            return "done";
        }
    }
}
