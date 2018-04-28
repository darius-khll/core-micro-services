using Microsoft.EntityFrameworkCore;
using System;

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
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //should not override for production environment
            //just for migration and development
            if (String.IsNullOrEmpty(env) || env.ToLower() != "production")
            {
                //could be UseSqlServer
                optionsBuilder.UseNpgsql(@"Server=localhost; Port=8189; User Id=user; Password=password; database=AppDbContext");
            }
        }
    }
}
