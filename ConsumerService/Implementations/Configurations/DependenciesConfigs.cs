using Autofac;
using Common.Implementations;
using ConsumerService.Consumers;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsumerService.Implementations.Configurations
{
    public class DependenciesConfigs
    {
        public ContainerBuilder InitializedDependencies(IConfigurationRoot configuration, ConsumerOptions consumerOptions)
        {
            ContainerBuilder builder = new ContainerBuilder();
            
            //"mongodb://user:password@localhost"
            builder.Register(ctx =>
            {
                return new MongoClient($"mongodb://{consumerOptions.MongoHost}").GetDatabase("secondDb");
            }).As<IMongoDatabase>();

            builder.RegisterType<HttpClient>().InstancePerLifetimeScope();
            builder.RegisterType<HttpService>().As<IHttpService>().InstancePerLifetimeScope();

            builder.RegisterType<DataAddedConsumer>();
            builder.RegisterType<PubSubConsumer>();
            builder.RegisterType<SubmitOrderConsumer>();


            builder.RegisterConsumers(Assembly.GetExecutingAssembly()); //register consumers

            return builder;
        }

        public void StartUpConfigurations(ContainerBuilder builder, ConsumerOptions consumerOptions)
        {
            builder.Register(context =>
            {
                var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri($"rabbitmq://{consumerOptions.RabbitHost}/"), h =>
                    {
                        h.Username(consumerOptions.RabbitUser);
                        h.Password(consumerOptions.RabbitPassword);
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
        }

        public async Task RunConsumer(ContainerBuilder builder)
        {
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
