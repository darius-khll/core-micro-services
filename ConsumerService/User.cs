using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsumerService
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
