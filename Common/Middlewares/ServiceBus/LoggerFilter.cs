using GreenPipes;
using MassTransit.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Middlewares.ServiceBus
{
    public class LoggerSpecification<T> : IPipeSpecification<T> where T : class, PipeContext
    {
        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        public void Apply(IPipeBuilder<T> builder)
        {
            builder.AddFilter(new LoggerFilter<T>());
        }
    }


    public class LoggerFilter<T> : IFilter<T> where T : class, PipeContext
    {
        public void Probe(ProbeContext context)
        {
        }

        public async Task Send(T context, IPipe<T> next)
        {
            Uri routeName = (Uri)(typeof(ConsumeContextProxy)
                .GetProperty(nameof(ConsumeContextProxy.DestinationAddress))
                .GetValue(context, null));

            int random = new Random().Next(1000);

            Console.WriteLine($"request {random}: '{routeName.LocalPath}' started!");
            
            await next.Send(context);

            Console.WriteLine($"request {random}: '{routeName.LocalPath}' finished!");

        }
    }

    public interface IMyInterface : PipeContext
    {
        string DestinationAddress { get; }
    }
}
