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
        public readonly IDapperRepository<Employee> _dapperRepository;
        public readonly AppDbContext _dbContext;

        public PostgresBusiness(IDapperRepository<Employee> dapperRepository, AppDbContext dbContext)
        {
            _dapperRepository = dapperRepository;
            _dbContext = dbContext;
        }


        public async Task HandleDapperLogic()
        {
            Guid id = Guid.NewGuid();
            var emp = new Employee { Id = id, first_name = "aaa11", last_name = "bbb11" };
            await _dapperRepository.Add(emp);

            emp.address = "tehran";
            await _dapperRepository.Update(emp);

            await _dapperRepository.Commit();

            Employee given = await _dapperRepository.FindById(id, nameof(Employee));

            await _dapperRepository.Remove(emp);

            await _dapperRepository.Commit();
        }

        public async Task HandleEfCoreLogic()
        {
            _dbContext.Users.Add(new Common.Repositories.Postgres.EfCore.User { Id = Guid.NewGuid(), Name = "aaa" });
            await _dbContext.SaveChangesAsync();
        }
    }
}
