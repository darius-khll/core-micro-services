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
        public static void AddConsumersEndpoint(this IRabbitMqBusFactoryConfigurator cfg, IRabbitMqHost host, IComponentContext context, string[] namespaces)
        {
            var consumerMethod = typeof(AutofacExtensions).GetTypeInfo().GetMethod(nameof(AutofacExtensions.Consumer), new[] { typeof(IReceiveEndpointConfigurator).GetTypeInfo(), typeof(IComponentContext).GetTypeInfo(), typeof(string).GetTypeInfo() });

            List<Type> entities = AppDomain.CurrentDomain.GetAssemblies()
               .Where(asm => asm.IsDynamic == false)
               //.Where(asm => asm.GetReferencedAssemblies().Any(refAsm => ))
               .SelectMany(x => x.GetExportedTypes())
               .Where(x => typeof(IConsumer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
               .Where(x => namespaces.Contains(x.Namespace))
               .ToList();

            foreach (var item in entities)
            {
                cfg.ReceiveEndpoint(host, item.Name, e =>
                {
                    consumerMethod.MakeGenericMethod(item).Invoke(e, new object[] { e, context, "message" });
                });
            }
        }
    }
}
