using Autofac;
using Common.Implementations;
using Common.Repositories.Mongo;
using Common.Repositories.Postgres.Dapper;
using ConsumerService.Business;
using ConsumerService.Consumers;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsumerService.Configurations
{
    public class DependenciesConfigs
    {
        public ContainerBuilder InitializedDependencies(IConfigurationRoot configuration, ConsumerOptions consumerOptions)
        {
            ContainerBuilder builder = new ContainerBuilder();

            //"mongodb://user:password@localhost"
            builder.Register(ctx => new MongoDataContext("mongoDatabase", $"mongodb://{consumerOptions.MongoHost}")).As<IMongoDataContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(MongoRepository<>)).As(typeof(IMongoRepository<>)).InstancePerLifetimeScope();


            string[] postgres = consumerOptions.PostgressHost.Split(":"); //0: host, 1: port
            //Server=localhost; Port=8189; User Id=postgres; Password=; Database=mydb
            string dapperConnetion = $"Server={postgres[0]}; Port={postgres[1]}; User Id=postgres; Password=; Database={consumerOptions.PostgresDbName}";
            builder.Register(ctx => new NpgsqlConnection(dapperConnetion)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(DapperRepository<>)).As(typeof(IDapperRepository<>)).InstancePerLifetimeScope();


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


                    cfg.AddConsumersEndpoint(host, context, new string[] { "ConsumerService.Consumers" }, (t, e) =>
                    {
                        //if (t == typeof(PubSubConsumer))
                        //{
                        //    return false;
                        //}

                        //e.UseRetry(r => r.Immediate(5));

                        if (t == typeof(DataAddedConsumer))
                        {
                            e.UseRetry(r => r.Immediate(1));
                        }

                        return true;
                    });


                    //cfg.ReceiveEndpoint(host, nameof(PubSubConsumer), e => e.Consumer<PubSubConsumer>(context));
                    //cfg.ReceiveEndpoint(host, nameof(SubmitOrderConsumer), e => e.Consumer<SubmitOrderConsumer>(context));
                    //cfg.ReceiveEndpoint(host, nameof(DataAddedConsumer), e =>
                    //{
                    //    e.UseRetry(r => r.Immediate(1));
                    //    e.Consumer<DataAddedConsumer>(context);
                    //    e.Consumer<DataAddedConsumerFault>(context);
                    //});

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
