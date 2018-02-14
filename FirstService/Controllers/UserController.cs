using FirstService.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    public class UserController : Controller
    {
        private readonly IFirstBusiness _firstBusiness;

        public UserController(IFirstBusiness firstBusiness)
        {
            _firstBusiness = firstBusiness;
        }

        public async Task<string> Index()
        {
            string name = await _firstBusiness.UserHandlerAsync("aaa", "bbb");
            return name;
        }

    }
}