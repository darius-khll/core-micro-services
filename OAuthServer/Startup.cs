using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OAuthServer.Configurations;
using System.Security.Cryptography.X509Certificates;

namespace OAuthServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //C:\Users\ali\source\repos\CoreMicroServices\OAuthServer\socialnetwork.pfx

            //openssl req -newkey rsa:2048 -nodes -keyout socialnetwok.key -x509 -days 365 -out socialnetwork.cer
            //you may need try this command befor execute next one => winpty bash
            //openssl pkcs12 -export -in socialnetwork.cer - inkey socialnetwok.key -out socialnetwork.pfx
            services.AddIdentityServer()
                //.AddDeveloperSigningCredential()
                .AddSigningCredential(new X509Certificate2(@"/app/socialnetwork.pfx", "password")) //password given from pfx file which genrated by cli
                .AddTestUsers(InMemoryConfiguration.Users())
                .AddInMemoryClients(InMemoryConfiguration.Clients())
                .AddInMemoryApiResources(InMemoryConfiguration.ApiResources());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole();
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }
    }
}
