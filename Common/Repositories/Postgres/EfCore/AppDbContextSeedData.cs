using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Repositories.Postgres.EfCore
{
    public static class AppDbContextSeedData
    {
        public static void SeedData(DbContextOptionsBuilder<AppDbContext> val)
        {
            using (var context = new AppDbContext(val.Options))
            {
                context.Database.EnsureCreated();

                if (!context.Users.Any())
                {
                    var persons = new List<User>
                    {
                        new User
                        {
                            Id = Guid.NewGuid(),
                            Name = "Admin",
                            Family = "User"
                        }
                    };
                    context.AddRange(persons);
                    context.SaveChanges();
                }
            }
        }
    }
}