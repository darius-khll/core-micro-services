using Autofac;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Implementations
{
    public static class ServiceBusExtensions
    {
        /*
         *  string[] namespaces => find consumer in specefic namespaces 
         *  endpoint should be name of Consumer
         */
        public static void AddConsumersEndpoint(this IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host, IComponentContext context, string[] namespaces, Func<TypeInfo, IRabbitMqReceiveEndpointConfigurator, bool> onConfigure = null)
        {
            MethodInfo autofacConsumerMethod = typeof(AutofacExtensions).GetTypeInfo().GetMethod(nameof(AutofacExtensions.Consumer), new[] { typeof(IReceiveEndpointConfigurator).GetTypeInfo(), typeof(IComponentContext).GetTypeInfo(), typeof(string).GetTypeInfo() });

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
    }
}
