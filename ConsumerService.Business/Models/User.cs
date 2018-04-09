using Common.Repositories.Mongo;
using System.Collections.Generic;

namespace ConsumerService.Business.Models
{
    public class User : IMongoEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Addresses { get; set; }
    }
}
