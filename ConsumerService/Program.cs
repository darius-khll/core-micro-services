using Autofac;
using Common.Implementations;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsumerService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine(configuration["redis:host"]);

            Rabbitmq(configuration).GetAwaiter().GetResult();
        }

        static async Task Rabbitmq(IConfigurationRoot configuration)
        {
            var builder = new ContainerBuilder();

            string mongoHost = configuration[$"{MongoOptions.GetConfigName}:{nameof(MongoOptions.host)}"];
            string rabbitHost = configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.host)}"];
            string rabbitUser = configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.user)}"];
            string rabbitPassword = configuration[$"{RabbitmqOptions.GetConfigName}:{nameof(RabbitmqOptions.password)}"];

            //"mongodb://user:password@localhost"
            builder.Register(ctx =>
            {
                return new MongoClient($"mongodb://{mongoHost}").GetDatabase("secondDb");
            }).As<IMongoDatabase>();

            builder.RegisterType<HttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<HttpService>().As<IHttpService>().InstancePerLifetimeScope();

            builder.RegisterType<DataAddedConsumer>();
            builder.RegisterType<PubSubConsumer>();
            builder.RegisterType<SubmitOrderConsumer>();


            builder.RegisterConsumers(Assembly.GetExecutingAssembly());
            builder.Register(context =>
            {
                var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{rabbitHost}/"), h =>
                    {
                        h.Username(rabbitUser);
                        h.Password(rabbitPassword);
                    });

                    cfg.ReceiveEndpoint(host, "order-service", e => e.Consumer<SubmitOrderConsumer>(context));
                    cfg.ReceiveEndpoint(host, "pub-sub", e => e.Consumer<PubSubConsumer>(context));
                    cfg.ReceiveEndpoint(host, "data-added", e =>
                    {
                        e.UseRetry(r => r.Immediate(5)); //retry 5times if exception happens
                        e.Consumer<DataAddedConsumer>(context);
                        e.Consumer<DataAddedFaultConsumer>(context); //when something bad happens
                    });
                });

                return bus;
            })
                .SingleInstance()
                .As<IBusControl>()
                .As<IBus>();

            var container = builder.Build();
            var bc = container.Resolve<IBusControl>();

            try
            {
                await bc.StartAsync();
                Console.WriteLine("Working....");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                //await bc.StopAsync();
            }
        }
    }
}
