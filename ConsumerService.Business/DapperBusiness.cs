using Common.Repositories.Postgres.Dapper;
using ConsumerService.Business.Models;
using System;
using System.Threading.Tasks;

namespace ConsumerService.Business
{

    public interface IDapperBusiness
    {
        Task HandleLogic();
    }

    public class DapperBusiness : IDapperBusiness
    {
        public readonly IDapperRepository<Employee> _dapperRepository;

        public DapperBusiness(IDapperRepository<Employee> dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task HandleLogic()
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
    }
}
