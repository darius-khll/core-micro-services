using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Redis;
using System.Net.Http;

namespace FirstService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                string foo = null;
                RedisManagerPool manager = new RedisManagerPool("redis");
                using (var client = manager.GetClient())
                {
                    client.Set("foo", "bar");
                    foo = client.Get<string>("foo");
                }

                using (HttpClient client = new HttpClient())
                {
                    var res = await client.GetAsync("http://secondService");
                    string content = await res.Content.ReadAsStringAsync();
                    await context.Response.WriteAsync($"First Service requested: {content} - {foo}");
                }

            });
        }
    }
}
