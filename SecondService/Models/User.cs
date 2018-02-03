using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SecondService.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
