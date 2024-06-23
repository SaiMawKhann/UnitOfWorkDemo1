using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Models;

namespace UnitOfWorkDemo.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        IReaderRepository<Product> ReaderProducts { get; }
        Task<int> CompleteAsync();
        Task<int> CompleteAsyncForReader();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        IReaderRepository<TEntity> GetReaderRepository<TEntity>() where TEntity : class;
    }
}
