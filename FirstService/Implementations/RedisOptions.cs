using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstService.Implementations
{
    public class RedisOptions
    {
        public string name { get; set; } = "localhost";

        public int port { get; set; } = 6379;
    }
}
