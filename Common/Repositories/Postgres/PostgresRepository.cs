using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using System;
using System.Data;
using System.Threading.Tasks;

/// <summary>
/// using dapper contrib
/// </summary>
namespace Common.Repositories.Postgres
{
    public interface IRepository<T> where T : IAggregateRoot
    {
        Task Add(T item);
        Task Remove(T item);
        Task Update(T item);
        Task<T> FindById(Guid id);
    }

    public interface IAggregateRoot
    {
        Guid Id { get; set; }
    }

    public abstract class Repository<T> : IRepository<T> where T : class, IAggregateRoot
    {

        internal NpgsqlConnection Connection
        {
            get
            {
                return new NpgsqlConnection("Server=localhost; Port=8189; User Id=postgres; Password=; Database=mydb");
            }
        }

        public virtual async Task Add(T item)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                if (item.Id != Guid.Empty)
                    item.Id = Guid.NewGuid();

                await cn.InsertAsync(item);
            }
        }

        public virtual async Task Update(T item)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                await cn.UpdateAsync(item);
            }
        }

        public virtual async Task Remove(T item)
        {
            using (IDbConnection cn = Connection)
            {
                cn.Open();
                await cn.DeleteAsync(item);
            }
        }

        public virtual async Task<T> FindById(Guid id)
        {
            T item = default(T);

            using (IDbConnection cn = Connection)
            {
                cn.Open();
                item = await cn.QuerySingleAsync<T>("SELECT * FROM " + nameof(T) + " WHERE Id=@Id", new { Id = id });
            }

            return item;
        }
    }

    public class PostgresRepository
    {
        public void f1()
        {
            using (var connection = new NpgsqlConnection("Server=localhost; Port=8189; User Id=postgres; Password=; Database=mydb"))
            {
                connection.Open();
                //connection.Execute("Insert into Employee (first_name, last_name, address) values ('John', 'Smith', '123 Duane St');");
                //var value = connection.Query<string>("Select first_name from Employee;");
                //Console.WriteLine(value.First());

                var a = new Employee { Id = Guid.NewGuid(), first_name = "ali", last_name = "kh" };

                connection.Insert(a);

                a.last_name = "khalili";

                connection.Update(a);
            }
        }
    }

    public class Employee : IAggregateRoot
    {
        [ExplicitKey]
        public Guid Id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address { get; set; }
    }
}
