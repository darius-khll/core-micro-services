using MongoDB.Driver;

namespace Common.Repositories.Mongo
{
    public interface IMongoDataContext
    {
        IMongoDatabase MongoDatabase { get; }
    }
    public class MongoDataContext : IMongoDataContext
    {
        public MongoDataContext(string databaseName, string connectionName = "mongodb://user:password@localhost")
        {
            IMongoClient client = new MongoClient(connectionName);
            MongoDatabase = client.GetDatabase(databaseName);
        }

        public IMongoDatabase MongoDatabase { get; }
    }
}
