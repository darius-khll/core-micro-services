using FirstService.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [Route("[controller]")]
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
    }
}
