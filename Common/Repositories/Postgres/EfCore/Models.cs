using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repositories.Postgres.EfCore
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
    }
    public class Address
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
    }
}
