using kzy_entities.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using kzy_entities.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace UnitOfWorkDemo.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query(
            Expression<Func<T, bool>>? expression = null,
            bool reader = false);

        IQueryable<T> SqlQuery(FormattableString query);

        T? Get(
            object id,
            bool reader = false);
        Task<T?> GetAsync(
            object id,
            bool reader = false,
            CancellationToken cancellationToken = default);

        T? Single(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false);
        Task<T?> SingleAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false,
            CancellationToken cancellationToken = default);

        T? First(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false);

        Task<T?> FirstAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false,
            CancellationToken cancellationToken = default);

        void Add(T entity);
        void Add(IEnumerable<T> entities);
        Task AddAsync(T entity);
        Task AddAsync(IEnumerable<T> entities);

        void Update(T entity);
        void Update(IEnumerable<T> entities);

        int BulkUpdate(
            Expression<Func<T, bool>> expression,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls);
        Task<int> BulkUpdateAsync(
            Expression<Func<T, bool>> expression,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls,
            CancellationToken cancellationToken = default);

        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteAsync(IEnumerable<T> entities);

        int BulkDelete(Expression<Func<T, bool>> expression);
        Task<int> BulkDeleteAsync(
            Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken = default);

        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default);
    }

    public class Repository<T> : IDisposable, IAsyncDisposable, IRepository<T> where T : class
    {
        protected readonly DbContext? _dbReaderContext;
        protected readonly DbSet<T>? _dbReaderSet;
        protected readonly DbContext _dbWriterContext;
        protected readonly DbSet<T> _dbWriterSet;
        //protected readonly DbContext _dbContext;
        //protected readonly DbSet<T> _dbSet;

        public Repository(DbContext writerContext, DbContext? readerContext = null)
        {
            _dbWriterContext = writerContext ?? throw new ArgumentException(nameof(writerContext));
            _dbReaderContext = readerContext;
            _dbWriterSet = _dbWriterContext.Set<T>();
            if (_dbReaderContext != null)
                _dbReaderSet = _dbReaderContext.Set<T>();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>>? expression = null, bool reader = false)
        {
            IQueryable<T> query;
            if (reader && _dbReaderSet != null)
                query = _dbReaderSet.AsNoTracking();
            else
                query = _dbWriterSet;

            if (expression != null)
                query = query.Where(expression);

            return query;
        }

        public IQueryable<T> SqlQuery(FormattableString query)
        {
            if (_dbReaderContext != null)
                return _dbReaderContext.Database.SqlQuery<T>(query);

            return _dbWriterContext.Database.SqlQuery<T>(query);
        }

        public int BulkUpdate(
            Expression<Func<T, bool>> expression,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls)
        {
            return BulkUpdateAsync(expression, setPropertyCalls).Result;
        }

        public async Task<int> BulkUpdateAsync(
            Expression<Func<T, bool>> expression,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls,
            CancellationToken cancellationToken = default)
        {
            return await _dbWriterSet
                .Where(expression)
                .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        }

        public int BulkDelete(Expression<Func<T, bool>> expression)
        {
            return BulkDeleteAsync(expression).Result;
        }

        public async Task<int> BulkDeleteAsync(
            Expression<Func<T, bool>> expression,
            CancellationToken cancellationToken = default)
        {
            return await _dbWriterSet
                .Where(expression)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public T? Get(object id, bool reader = false)
        {
            return GetAsync(id, reader).Result;
        }

        public async Task<T?> GetAsync(
            object id,
            bool reader = false,
            CancellationToken cancellationToken = default)
        {
            if (reader && _dbReaderSet != null)
                return await _dbReaderSet.FindAsync(id, cancellationToken);

            return await _dbWriterSet.FindAsync(id, cancellationToken);
        }

        public T? Single(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false)
        {
            return SingleAsync(predicate, orderBy, includes, reader).Result;
        }

        public async Task<T?> SingleAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query;
            if (reader && _dbReaderSet != null)
                query = _dbReaderSet.AsNoTracking();
            else
                query = _dbWriterSet;

            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).SingleOrDefaultAsync(cancellationToken);

            return await query.SingleOrDefaultAsync(cancellationToken);
        }

        public T? First(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false)
        {
            return FirstAsync(predicate, orderBy, includes, reader).Result;
        }

        public async Task<T?> FirstAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null,
            bool reader = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query;
            if (reader && _dbReaderSet != null)
                query = _dbReaderSet.AsNoTracking();
            else
                query = _dbWriterSet;

            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).FirstOrDefaultAsync(cancellationToken);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public void Add(T entity)
        {
            _dbWriterSet.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _dbWriterSet.AddAsync(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            _dbWriterSet.AddRange(entities);
        }

        public async Task AddAsync(IEnumerable<T> entities)
        {
            await _dbWriterSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbWriterSet.Update(entity);
        }

        public void Update(IEnumerable<T> entities)
        {
            _dbWriterSet.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            _dbWriterSet.Remove(entity);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbWriterSet.RemoveRange(entities);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbWriterSet.Remove(entity);
            await _dbWriterContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(IEnumerable<T> entities)
        {
            _dbWriterSet.RemoveRange(entities);
            await _dbWriterContext.SaveChangesAsync();
        }

        public void Save()
        {
            _dbWriterContext.SaveChanges();
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            await _dbWriterContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }

        public async ValueTask DisposeAsync()
        {
            if (_dbWriterContext != null)
                await _dbWriterContext.DisposeAsync();

            if (_dbReaderContext != null)
                await _dbReaderContext.DisposeAsync();

            //if (_dbContext != null)
            //    await _dbContext.DisposeAsync();
        }
    }

}
