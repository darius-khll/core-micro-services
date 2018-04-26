using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Repositories.Postgres.EfCore
{
    public static class AppDbContextSeedData
    {
        public static void SeedData(DbContextOptions<AppDbContext> options)
        {
            using (var context = new AppDbContext(options))
            {
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