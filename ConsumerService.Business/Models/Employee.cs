using Common.Repositories.Postgres.Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsumerService.Business.Models
{
    public class Employee : IDapperEntity
    {
        //uuid type in postgres
        [ExplicitKey]
        public Guid Id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address { get; set; }
    }
}
