using Autofac;
using Common.Implementations;
using GreenPipes;
using MassTransit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsumerService
{
    class Program
    {
        static void Main(string[] args)
        {
            Rabbitmq().GetAwaiter().GetResult();
        }

        static async Task Rabbitmq()
        {
            var builder = new ContainerBuilder();

            //"mongodb://user:password@localhost"
            builder.Register(ctx =>
            {
                return new MongoClient("mongodb://mongo");
            }).As<IMongoClient>();

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
                    var host = cfg.Host(new Uri("rabbitmq://rabbitmq:5672/"), h =>
                    {
                        h.Username("user");
                        h.Password("password");
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

        async Task Mongo()
        {
            //"mongodb://user:password@localhost"
            IMongoClient client = new MongoClient("mongodb://mongo"); //The client handles and dispose it automatically
            IMongoDatabase db = client.GetDatabase("secondDb");

            await db.GetCollection<User>("Users").InsertOneAsync(new User { Name = "abc1", Age = 10 });
            List<User> users = (await db.GetCollection<User>("Users").FindAsync(c => c.Name == "abc1")).ToList();

            Console.WriteLine(users[0].Name);
        }
    }
}
