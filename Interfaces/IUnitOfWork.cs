using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Models;

namespace UnitOfWorkDemo.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        Task<int> CompleteAsync();
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}
