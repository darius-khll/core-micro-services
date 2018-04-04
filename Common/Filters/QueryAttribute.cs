using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Common.Filters
{
    public class QueryAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
