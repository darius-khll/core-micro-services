using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.Repositories
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public interface IEntity : IEntity<string>
    {
    }

    public interface IRepository<TEntity, in TKey> where TEntity : IEntity<TKey>
    {
        Task<TEntity> GetByIdAsync(TKey id);

        Task<TEntity> SaveAsync(TEntity entity);

        Task DeleteAsync(TKey id);

        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

    }

    public abstract class BaseMongoRepository<TEntity>
        : IRepository<TEntity, string> where TEntity : IEntity
    {

        private readonly MongoDataContext _dataContext;

        public BaseMongoRepository()
        {
            // use it when you want add repository for each model and you can override below method 
            // protected virtual IMongoCollection<TEntity> Collection { get; private set; }
            // like this
            //protected override IMongoCollection<MyEntity> Collection => _dataContext.MongoDatabase.GetCollection<Person>(myCollectionName);
        }

        public BaseMongoRepository(MongoDataContext dataContext)
        {
            // use this constructor enable generic repository is ready to use for all entities, create collection name based on that entity
            // consider renaming entities could raise problem and change collection name
            _dataContext = dataContext;
            Collection = _dataContext.MongoDatabase.GetCollection<TEntity>(nameof(TEntity));
        }

        protected virtual IMongoCollection<TEntity> Collection { get; private set; }

        public virtual async Task<TEntity> GetByIdAsync(string id)
        {
            return await Collection.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public virtual async Task<TEntity> SaveAsync(TEntity entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Id))
            {
                entity.Id = ObjectId.GenerateNewId().ToString();
            }

            await Collection.ReplaceOneAsync(
                x => x.Id.Equals(entity.Id),
                entity,
                new UpdateOptions
                {
                    IsUpsert = true
                });

            return entity;
        }

        public virtual async Task DeleteAsync(string id)
        {
            await Collection.DeleteOneAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<ICollection<TEntity>> FindAllAsync(
            Expression<Func<TEntity, bool>> predicate)
        {
            return await Collection.Find(predicate).ToListAsync();
        }
    }


    public class MongoDataContext
    {
        public MongoDataContext(string databaseName, string connectionName = "mongodb://user:password@localhost")
        {
            IMongoClient client = new MongoClient(connectionName);
            MongoDatabase = client.GetDatabase(databaseName);
        }

        public IMongoDatabase MongoDatabase { get; }
    }



    public class PersonRepository : BaseMongoRepository<Person>
    {
        private const string PeopleCollectionName = "People";

        private readonly MongoDataContext _dataContext;

        public PersonRepository(MongoDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        protected override IMongoCollection<Person> Collection =>
            _dataContext.MongoDatabase.GetCollection<Person>(PeopleCollectionName);
    }
    public class Person : IEntity
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
