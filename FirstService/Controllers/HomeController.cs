using Microsoft.AspNetCore.Mvc;

namespace FirstService.Controllers
{
    public class HomeController : Controller
    {
        public string Error()
        {
            return "Error happened!";
        }
    }
}