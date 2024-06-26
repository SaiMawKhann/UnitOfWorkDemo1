using kzy_entities.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Interfaces;

namespace UnitOfWorkDemo.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> GGWPChangesAsync();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }

    public interface IUnitOfWork<T, U> : IUnitOfWork where T : DbContext where U : DbContext
    {
    }

    public class UnitOfWork<T, U> : IUnitOfWork<T, U> where T : DbContext where U : DbContext
    {
        private readonly ApplicationDbContext _dbWriterContext;
        private readonly ReaderDbContext _dbReaderContext;

        public UnitOfWork(ApplicationDbContext writerContext, ReaderDbContext readerContext)
        {
            _dbWriterContext = writerContext;
            _dbReaderContext = readerContext;
        }

        public async Task<int> GGWPChangesAsync()
        {
            var writerResult = await _dbWriterContext.SaveChangesAsync();
            return writerResult;
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
