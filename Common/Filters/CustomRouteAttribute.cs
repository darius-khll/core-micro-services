using Microsoft.AspNetCore.Mvc;

namespace Common.Filters
{
    public class CustomRouteAttribute : RouteAttribute
    {
        private static string _prefix = "/api/[controller]";
        public CustomRouteAttribute() : base(_prefix)
        {
        }
    }
}
