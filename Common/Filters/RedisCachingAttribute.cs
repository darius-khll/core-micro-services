using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common.Filters
{
    public class RedisCachingAttribute : Attribute, IAsyncActionFilter
    {
        public IDistributedCache _distributedCache { get; }
        public string _key { get; }

        public RedisCachingAttribute(IDistributedCache distributedCache, string key)
        {
            _distributedCache = distributedCache;
            _key = key;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var value = await _distributedCache.GetStringAsync(_key);

            if (value != null)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(value);
                await context.HttpContext.Response.Body.WriteAsync
                    (buffer, 0, buffer.Length);
                return;
            }
            else
            {
                string time = DateTime.Now.ToLongTimeString();
                await _distributedCache.SetStringAsync(_key, time);
            }
            var resultContext = await next();
        }
    }
}
