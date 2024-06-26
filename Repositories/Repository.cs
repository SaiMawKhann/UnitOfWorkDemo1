using kzy_entities.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using kzy_entities.Entities;
using UnitOfWorkDemo.Interfaces;

namespace UnitOfWorkDemo.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbWriterContext;
        private readonly ReaderDbContext _dbReaderContext;
        private readonly DbSet<T> _dbWriterSet;
        private readonly DbSet<T> _dbReaderSet;

        public Repository(ApplicationDbContext writerContext, ReaderDbContext readerContext)
        {
            _dbWriterContext = writerContext;
            _dbReaderContext = readerContext;
            _dbWriterSet = _dbWriterContext.Set<T>();
            _dbReaderSet = _dbReaderContext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool reader = false)
        {
            if (reader)
            {
                return await _dbReaderSet.ToListAsync();
            }
            return await _dbWriterSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id, bool reader = false)
        {
            if (reader)
            {
                return await _dbReaderSet.FindAsync(id);
            }
            return await _dbWriterSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbWriterSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbWriterSet.Attach(entity);
            _dbWriterContext.Entry(entity).State = EntityState.Modified;
            _dbReaderSet.Attach(entity);
            _dbReaderContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbWriterSet.Remove(entity);
            _dbReaderSet.Remove(entity);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> expression, bool reader = false)
        {
            if (reader)
            {
                return _dbReaderSet.Where(expression).AsNoTracking();
            }
            return _dbWriterSet.Where(expression);
        }
    }
}
