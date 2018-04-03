using FirstService.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Common.Filters;

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

        [HttpGet]
        [Route(nameof(Index))]
        public async Task<string> Index()
        {
            string result = await _cacheBusiness.CacheIfNotExist();
            return result;
        }

        [HttpGet]
        [Route(nameof(Del))]
        public async Task<string> Del()
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
