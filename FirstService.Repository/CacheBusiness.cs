using FirstService.Repository.Implementations;
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
        private readonly IRedisCaching<string> _redisCaching;
        private const string _key = "TheTime";

        public CacheBusiness(IRedisCaching<string> distributedCache)
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
