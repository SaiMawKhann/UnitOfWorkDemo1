// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using UnitOfWorkDemo.Models;
using UnitOfWorkDemo1.Models;

namespace UnitOfWorkDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
