using Common.Filters;
using ConsumerService.Business;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [CustomRoute]
    public class CacheController : Controller
    {
        private readonly ICacheBusiness _cacheBusiness;

        public CacheController(ICacheBusiness cacheBusiness)
        {
            _cacheBusiness = cacheBusiness;
        }

        /*
         *  get something from redis distributed cache &
         *  cache it
         */
        [HttpGet]
        [Route(nameof(CacheIfNotExist))]
        public async Task<string> CacheIfNotExist()
        {
            string result = await _cacheBusiness.CacheIfNotExist();
            return result;
        }

        /*
         *  remove from distributed cache
         */
        [HttpGet]
        [Route(nameof(RemoveCache))]
        public async Task<string> RemoveCache()
        {
            string result = await _cacheBusiness.RemoveCache();
            return result;
        }

        [HttpGet]
        [Route(nameof(CacheSample))]
        //[ServiceFilter(typeof(RedisCachingAttribute))] => it needs to added RedisCachingAttribute to DI services too (per scope)
        [TypeFilter(typeof(RedisCachingAttribute), Arguments = new object[] { "my value" })] // it does not need to added to DI container 
        public async Task<string> CacheSample()
        {
            return await Task.FromResult("1");
        }
    }
}
