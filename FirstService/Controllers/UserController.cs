using Common.Filters;
using FirstService.Repository;
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

        [HttpGet]
        [Route(nameof(Index))]
        public async Task<string> Index()
        {
            string name = await _firstBusiness.UserHandlerAsync("aaa", "bbb");
            return name;
        }

        [HttpGet]
        [Route(nameof(GetUser))]
        public async Task<string> GetUser()
        {
            string user = await _firstBusiness.UserTest("aaa", "bbb");
            return user;
        }

    }
}