using kzy_entities.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace UnitOfWorkDemo.Repositories
{
    #region Interfaces
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWork<T, U> : IUnitOfWork where T : DbContext where U : DbContext
    {

    }
    #endregion
    #region Implementations
    public class UnitOfWork<T, U> : IUnitOfWork<T, U> where T : DbContext where U : DbContext
    {
        private Dictionary<Type, object> _repositories;

        public UnitOfWork(T writerConext, U readerContext)
        {
            _repositories ??= new Dictionary<Type, object>();

            WriterContext = writerConext ?? throw new ArgumentNullException(nameof(writerConext));
            ReaderContext = readerContext;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type)) _repositories[type] = new Repository<TEntity>(WriterContext, ReaderContext);
            return (IRepository<TEntity>)_repositories[type];
        }
        public U ReaderContext { get; }
        public T WriterContext { get; }

        public int SaveChanges()
        {
            return WriterContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await WriterContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            if (WriterContext != null)
            {
                await WriterContext.DisposeAsync();
            }

            if (ReaderContext != null)
            {
                await ReaderContext.DisposeAsync();
            }
        }
    }
    #endregion
}
