using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitOfWorkDemo.Data;
using UnitOfWorkDemo.Interfaces;

namespace UnitOfWorkDemo.Repositories
{
    public class ReaderRepository<T> : IReaderRepository<T> where T : class
    {
        private readonly ReaderDbContext _readerContext;

        public ReaderRepository(ReaderDbContext readerContext)
        {
            _readerContext = readerContext;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _readerContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _readerContext.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _readerContext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _readerContext.Set<T>().Attach(entity);
            _readerContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _readerContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return _readerContext.Set<T>().Where(predicate);
        }
    }
}
