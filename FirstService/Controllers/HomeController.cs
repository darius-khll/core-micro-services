using FirstService.Repository.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FirstService.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpService _httpService;

        public HomeController(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public string Error()
        {
            return "Error happened!";
        }

        [Authorize]
        public string Valid()
        {
            var claims = User.Claims;
            return "the user is authenticated";
        }

        public async Task<string> GetToken()
        {
            var obj = new { client_id = "socialnetwork", username = "username", password = "password", client_secret = "secret", grant_type = "password" };

            var res = await _httpService.PostAsync("http://oauthserver/connect/token", obj);
            return res;
        }
    }
}