using Autofac;
using Common.Implementations;
using Common.Repositories.Mongo;
using ConsumerService.Business;
using ConsumerService.Consumers;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
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
            builder.Register(ctx => new MongoDataContext("mongoDatabase", $"mongodb://{consumerOptions.MongoHost}")).As<IMongoDataContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(MongoRepository<>)).As(typeof(IMongoRepository<>)).InstancePerLifetimeScope();

            builder.RegisterType<HttpClient>().SingleInstance();
            builder.RegisterType<HttpService>().As<IHttpService>().SingleInstance();

            /* register all Consumers instead of
            builder.RegisterType<DataAddedConsumer>(); and etc ... */
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).Where(t => t.Name.EndsWith("Consumer")).AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(MongoBusiness).Assembly).Where(t => t.Name.EndsWith("Business")).AsImplementedInterfaces().InstancePerLifetimeScope();

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


                    //cfg.ReceiveEndpoint(host, nameof(DataAddedConsumer), e =>
                    //{
                    //    e.UseRetry(r => r.Immediate(3));
                    //    e.Consumer<DataAddedConsumer>(context);
                    //    e.Consumer<DataAddedConsumerFault>(context);
                    //});


                    cfg.AddConsumersEndpoint(host, context, new string[] { "ConsumerService.Consumers" });

                    /*
                    cfg.ReceiveEndpoint(host, "pub-sub", e => e.Consumer<PubSubConsumer>(context));
                    cfg.ReceiveEndpoint(host, "data-added", e =>
                    {
                        e.UseRetry(r => r.Immediate(5)); //retry 5times if exception happens
                        e.Consumer<DataAddedConsumer>(context);
                        e.Consumer<DataAddedFaultConsumer>(context); //when something bad happens
                    });
                    */
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
