using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SecondService.Models;
using System.Collections.Generic;

namespace SecondService
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
                //"mongodb://user:password@localhost"
                MongoClient client = new MongoClient("mongodb://mongo"); //The client handles and dispose it automatically
                IMongoDatabase db = client.GetDatabase("secondDb");

                await db.GetCollection<User>("Users").InsertOneAsync(new User { Name = "abc1", Age = 10 });
                List<User> users = (await db.GetCollection<User>("Users").FindAsync(c => c.Name == "abc1")).ToList();

                await context.Response.WriteAsync($"Second Service respond a user named: {users[0].Name}");
            });
        }
    }
}
