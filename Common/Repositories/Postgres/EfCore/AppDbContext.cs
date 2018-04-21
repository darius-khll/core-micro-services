using Microsoft.EntityFrameworkCore;
using System;

namespace Common.Repositories.Postgres.EfCore
{
    //dotnet ef --startup-project ../MyProjectWithStartuo/ migrations add CreateDatabase
    //dotnet ef  migrations add CreateDatabase
    //dotnet ef database update
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Server=localhost; Port=8189; User Id=postgres; Password=; database=AppDbCcontext");
        }
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
