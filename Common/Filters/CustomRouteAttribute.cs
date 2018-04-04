using Microsoft.AspNetCore.Mvc;

namespace Common.Filters
{
    public class CustomRouteAttribute : RouteAttribute
    {
        public CustomRouteAttribute(string prefix = "/api/[controller]")
            : base(prefix)
        {
        }
    }
}
