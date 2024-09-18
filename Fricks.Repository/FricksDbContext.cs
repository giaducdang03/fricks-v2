using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Repository
{
    public class FricksDbContext : DbContext
    {
        public FricksDbContext() { }
        public FricksDbContext(DbContextOptions<FricksDbContext> options) : base(options) { }

        //Add DbSet
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Add FluentAPI
            base.OnModelCreating(modelBuilder);
        }
    }
}
