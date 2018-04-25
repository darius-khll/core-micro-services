using GreenPipes;
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
            Console.WriteLine("--------------------");
            Console.WriteLine("request started!");
            //string a = ((IMyInterface)context).DestinationAddress;

            await next.Send(context);

            Console.WriteLine("request finished!");
            Console.WriteLine("--------------------");

        }
    }

    public interface IMyInterface : PipeContext
    {
        string DestinationAddress { get; }
    }
}
