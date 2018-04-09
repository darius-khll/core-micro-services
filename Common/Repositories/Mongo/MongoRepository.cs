using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.Repositories.Mongo
{
    public interface IMongoRepository<TEntity> where TEntity : IMongoEntity
    {
        Task<TEntity> GetByIdAsync(string id);

        Task<TEntity> SaveAsync(TEntity entity);

        Task DeleteAsync(string id);

        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate);

    }

    public class MongoRepository<TEntity>
        : IMongoRepository<TEntity> where TEntity : IMongoEntity
    {

        private readonly IMongoDataContext _dataContext;

        public MongoRepository(IMongoDataContext dataContext)
        {
            _dataContext = dataContext;

            // use this constructor enable generic repository is ready to use for all entities, create collection name based on that entity
            // consider renaming entities could raise problem and change collection name
            MakeCollcetion();
        }

        protected virtual void MakeCollcetion()
        {
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
    
}
