using Microsoft.EntityFrameworkCore;
using UnitOfWorkDemo.Models; // Adjust namespace as per your application
using UnitOfWorkDemo1.Models;

public class ReaderDbContext : DbContext
{
    public ReaderDbContext(DbContextOptions<ReaderDbContext> options)
        : base(options)
    {
    }

    // DbSet properties for entities in the reader database
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
}
