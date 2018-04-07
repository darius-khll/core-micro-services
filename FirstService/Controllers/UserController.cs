using Common.Filters;
using ConsumerService.Business;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    [CustomRoute]
    public class UserController : Controller
    {
        private readonly IFirstBusiness _firstBusiness;

        public UserController(IFirstBusiness firstBusiness)
        {
            _firstBusiness = firstBusiness;
        }

        /*
         *  set user in redis
         *  get user in redis
         */
        [HttpGet]
        [Route(nameof(SetGetInRedis))]
        public async Task<string> SetGetInRedis()
        {
            string name = await _firstBusiness.UserHandlerAsync("aaa", "bbb");
            return name;
        }

    }
}