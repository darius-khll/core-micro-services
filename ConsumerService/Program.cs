using FirstService.Repository.Implementations;
using MassTransit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsumerService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://rabbitmq/"), h => { });

                cfg.ReceiveEndpoint(host, "order-service", e =>
                {
                    e.Handler<SubmitOrder>(context => context.RespondAsync<OrderAccepted>(new
                    {
                        context.Message.OrderId
                    }));
                });
            });

            await bus.StartAsync();
            try
            {
                Console.WriteLine("Working....");

                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                await bus.StopAsync();
            }
        }

        async Task Mongo()
        {
            //"mongodb://user:password@localhost"
            MongoClient client = new MongoClient("mongodb://mongo"); //The client handles and dispose it automatically
            IMongoDatabase db = client.GetDatabase("secondDb");

            await db.GetCollection<User>("Users").InsertOneAsync(new User { Name = "abc1", Age = 10 });
            List<User> users = (await db.GetCollection<User>("Users").FindAsync(c => c.Name == "abc1")).ToList();

            Console.WriteLine(users[0].Name);
        }
    }
}
