using FirstService.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
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

        [HttpGet]
        public async Task<string> Index()
        {
            string name = await _firstBusiness.UserHandlerAsync("aaa", "bbb");
            return "aaa";
        }

        [HttpGet]
        public string GetUser()
        {
            string user = _firstBusiness.UserTest("aaa", "bbb");
            return user;
        }

    }
}