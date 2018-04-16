using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using System;
using System.Threading.Tasks;

/// <summary>
/// using dapper contrib
/// </summary>
namespace Common.Repositories.Postgres.Dapper
{
    public interface IDapperRepository<T> where T : IDapperEntity
    {
        Task Add(T item);
        Task Remove(T item);
        Task Update(T item);
        Task<T> FindById(Guid Id, string tableName);
        Task Commit();
    }
    
    public class DapperRepository<T> : IDisposable, IDapperRepository<T> where T : class, IDapperEntity
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;
        public DapperRepository(NpgsqlConnection connection)
        {
            _connection = connection;
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }


        public virtual async Task Add(T item)
        {
            if (item.Id != Guid.Empty)
                item.Id = Guid.NewGuid();

            await _connection.InsertAsync(item, _transaction);
        }

        public virtual async Task Update(T item)
        {
            await _connection.UpdateAsync(item, _transaction);
        }

        public virtual async Task Remove(T item)
        {
            await _connection.DeleteAsync(item, _transaction);
        }

        public virtual async Task<T> FindById(Guid Id, string tableName)
        {
            T item = default(T);
            item = await _connection.QuerySingleAsync<T>($"SELECT * FROM {tableName}s WHERE \"Id\"=@Id", new { Id = Id }, _transaction);
            return item;
        }

        public virtual async Task Commit()
        {
            await _transaction.CommitAsync();
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _transaction?.Dispose();
        }
    }
}
