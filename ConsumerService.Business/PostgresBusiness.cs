using Common.Repositories.Postgres.Dapper;
using Common.Repositories.Postgres.EfCore;
using ConsumerService.Business.Models;
using System;
using System.Threading.Tasks;

namespace ConsumerService.Business
{

    public interface IPostgresBusiness
    {
        Task HandleDapperLogic();
        Task HandleEfCoreLogic();
    }

    public class PostgresBusiness : IPostgresBusiness
    {
        /// <summary>
        /// I used Lazy because I wanted to create instance and open Dapper connection and transaction after truly it's usage
        /// </summary>
        public readonly Lazy<IDapperRepository<Employee>> _dapperRepository;
        public readonly AppDbContext _dbContext;

        public PostgresBusiness(Lazy<IDapperRepository<Employee>> dapperRepository, AppDbContext dbContext)
        {
            _dapperRepository = dapperRepository;
            _dbContext = dbContext;
        }


        public async Task HandleDapperLogic()
        {
            IDapperRepository<Employee> _dapperRepositoryValue = _dapperRepository.Value;

            Guid id = Guid.NewGuid();
            var emp = new Employee { Id = id, first_name = "aaa11", last_name = "bbb11" };
            await _dapperRepositoryValue.Add(emp);

            emp.address = "tehran";
            await _dapperRepositoryValue.Update(emp);

            await _dapperRepositoryValue.Commit();

            Employee given = await _dapperRepositoryValue.FindById(id, nameof(Employee));

            await _dapperRepositoryValue.Remove(emp);

            await _dapperRepositoryValue.Commit();
        }

        public async Task HandleEfCoreLogic()
        {
            _dbContext.Users.Add(new Common.Repositories.Postgres.EfCore.User { Id = Guid.NewGuid(), Name = "aaa" });
            await _dbContext.SaveChangesAsync();
        }
    }
}
