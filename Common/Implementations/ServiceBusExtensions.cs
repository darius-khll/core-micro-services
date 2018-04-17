using Autofac;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Implementations
{
    public static class ServiceBusExtensions
    {
        /// <summary>
        ///     endpoint should be name of Consumer
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="host">host</param>
        /// <param name="context">context</param>
        /// <param name="namespaces">find consumer in specefic namespaces </param>
        /// <param name="onConfigure">overrided configs</param>
        public static void AddConsumersEndpoint(this IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host, IComponentContext context, string[] namespaces, Func<TypeInfo, IRabbitMqReceiveEndpointConfigurator, bool> onConfigure = null)
        {
            MethodInfo autofacConsumerMethod = typeof(AutofacExtensions).GetTypeInfo()
                .GetMethod(nameof(AutofacExtensions.Consumer), new[] { typeof(IReceiveEndpointConfigurator).GetTypeInfo(), typeof(IComponentContext).GetTypeInfo(), typeof(string).GetTypeInfo() });

            List<TypeInfo> consumersTypes = AppDomain.CurrentDomain.GetAssemblies()
               .Where(asm => asm.IsDynamic == false)
               .Where(asm => asm.GetReferencedAssemblies().Any(refAsm => refAsm.Name == "MassTransit"))
               .SelectMany(x => x.GetExportedTypes())
               .Where(x => typeof(IConsumer).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
               .Where(x => namespaces.Contains(x.Namespace))
               .Select(x => x.GetTypeInfo())
               .ToList();

            foreach (TypeInfo consumerType in consumersTypes)
            {
                cfg.ReceiveEndpoint(host, consumerType.Name, e =>
                {
                    bool shouldRegisterConsumer = false;
                    if (onConfigure == null)
                        shouldRegisterConsumer = true;
                    else
                        shouldRegisterConsumer = onConfigure.Invoke(consumerType, e);

                    if (shouldRegisterConsumer)
                        autofacConsumerMethod.MakeGenericMethod(consumerType).Invoke(e, new object[] { e, context, "message" });
                });
            }
        }

        public static void f1(IServiceCollection services, string rabbitmqHost, TimeSpan timeout, string[] namespaces)
        {

            List<TypeInfo> consumersTypes = AppDomain.CurrentDomain.GetAssemblies()
               .Where(asm => asm.IsDynamic == false)
               .Where(asm => asm.GetReferencedAssemblies().Any(refAsm => refAsm.Name == "MassTransit"))
               .SelectMany(x => x.GetExportedTypes())
               .Where(x => typeof(IRequestResponse).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract)
               .Where(x => namespaces.Contains(x.Namespace))
               .Select(x => x.GetTypeInfo())
               .ToList();

            foreach (TypeInfo consType in consumersTypes)
            {
                Type responseType = null, requestType = null;

                foreach (var genericType in consType.GetInterfaces())
                {

                    if (genericType.IsGenericType && genericType.GetGenericTypeDefinition() == typeof(IRequestResponse<>))
                    {
                        responseType = genericType.GetGenericArguments()[0]; //response type
                    }
                    else if (genericType.IsGenericType && genericType.GetGenericTypeDefinition() == typeof(IConsumer<>))
                    {
                        requestType = genericType.GetGenericArguments()[0]; //request type
                    }
                }

                Uri serviceAddress = new Uri($"rabbitmq://{rabbitmqHost}/{consType.Name}");

                var requestClientGenericType = typeof(IRequestClient<,>).MakeGenericType(new Type[] { requestType, responseType });

                ServiceCollectionServiceExtensions.AddScoped(services, requestClientGenericType, new Func<IServiceProvider, object>(sp =>
                {
                    return Activator.CreateInstance(typeof(MessageRequestClient<,>).MakeGenericType(requestType, responseType),
                        args: new object[] {
                            sp.GetRequiredService<IBus>(), serviceAddress, timeout, timeout
                        });
                }));
            }
        }
    }
}
