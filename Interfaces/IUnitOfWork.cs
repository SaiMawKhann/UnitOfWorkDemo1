using System;
using System.Threading.Tasks;
using UnitOfWorkDemo.Models;
using UnitOfWorkDemo.Repositories;

namespace UnitOfWorkDemo.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        Task<int> CompleteAsync();
        Repository<TEntity> GetRepository<TEntity>() where TEntity : class; // Added method
    }
}
