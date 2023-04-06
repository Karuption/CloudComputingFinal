using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CCFinal.Entities;

namespace CCFinal.Data
{
    public class CCFinalContext : DbContext
    {
        public CCFinalContext (DbContextOptions<CCFinalContext> options)
            : base(options)
        {
        }

        public DbSet<CCFinal.Entities.ToDoTask> ToDoTask { get; set; } = default!;

        public async Task Seed() {

            var seed = new List<ToDoTask>() {
                new(){Title = "Your first item!"}
            };

            ToDoTask.AddRangeAsync(seed);
        }
    }
}
