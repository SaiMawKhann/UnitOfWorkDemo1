using System.Threading.Tasks;
using UnitOfWorkDemo.Data;
using UnitOfWorkDemo.Interfaces;
using UnitOfWorkDemo.Models;

namespace UnitOfWorkDemo.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private Repository<Product> _productRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Product> Products => _productRepository ??= new Repository<Product>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Repository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_context); // Updated method
        }
    }
}
