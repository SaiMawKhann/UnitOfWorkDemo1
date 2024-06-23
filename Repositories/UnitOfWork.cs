using System;
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
        private IRepository<Product> _productRepository;

        public UnitOfWork(ApplicationDbContext writerContext, ReaderDbContext readerContext)
        {
            _dbWriterContext = writerContext;
            _dbReaderContext = readerContext;
        }

        public IRepository<Product> Products => _productRepository ??= new Repository<Product>(_dbWriterContext, _dbReaderContext);

        public async Task<int> CompleteAsync()
        {
            var writerResult = await _dbWriterContext.SaveChangesAsync();
            var readerResult = await _dbReaderContext.SaveChangesAsync();
            return writerResult + readerResult;
        }

        public void Dispose()
        {
            _dbWriterContext.Dispose();
            _dbReaderContext.Dispose();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_dbWriterContext, _dbReaderContext);
        }
    }
}
