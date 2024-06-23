using System.Threading.Tasks;
using UnitOfWorkDemo.Data;
using UnitOfWorkDemo.Interfaces;
using UnitOfWorkDemo.Models;

namespace UnitOfWorkDemo.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbWriterContext;
        private readonly ReaderDbContext _dbReaderContext;
        private Repository<Product> _productRepository;
        private ReaderRepository<Product> _readerProductRepository;

        public UnitOfWork(ApplicationDbContext writerContext, ReaderDbContext readerContext)
        {
            _dbWriterContext = writerContext;
            _dbReaderContext = readerContext;
        }

        public IRepository<Product> Products => _productRepository ??= new Repository<Product>(_dbWriterContext);
        public IReaderRepository<Product> ReaderProducts => _readerProductRepository ??= new ReaderRepository<Product>(_dbReaderContext);

        public async Task<int> CompleteAsync()
        {
            return await _dbWriterContext.SaveChangesAsync();
        }

        public async Task<int> CompleteAsyncForReader()
        {
            return await _dbReaderContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbWriterContext.Dispose();
            _dbReaderContext.Dispose();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_dbWriterContext);
        }

        public IReaderRepository<TEntity> GetReaderRepository<TEntity>() where TEntity : class
        {
            return new ReaderRepository<TEntity>(_dbReaderContext);
        }
    }
}
