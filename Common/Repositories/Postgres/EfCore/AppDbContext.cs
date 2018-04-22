using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Repositories.Postgres.EfCore
{
    //dotnet ef --startup-project ../ConsumerService/ migrations add CreateDatabase
    //dotnet ef  migrations add CreateDatabase
    //dotnet ef database update
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        //used for migrations enabled with overridnig OnConfiguring
        public AppDbContext() { }

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //for this scenario I create migration using this way but application works with options constructor
            //we use this way overriding OnConfiguring when we wanna hard coded connection string
            //used without DbContextOptions
            //could be UseSqlServer
            optionsBuilder.UseNpgsql(@"Server=localhost; Port=8189; User Id=postgres; Password=; database=AppDbContext");
        }
    }

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
