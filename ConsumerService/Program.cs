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
        static void Main(string[] args)
        {
            Rabbitmq().GetAwaiter().GetResult();
        }

        static async Task Rabbitmq()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://rabbitmq:5672/"), h =>
                {
                    h.Username("user");
                    h.Password("password");
                });

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
