﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitOfWorkDemo.Data;
using UnitOfWorkDemo.Interfaces;

namespace UnitOfWorkDemo.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbWriterContext;

        public Repository(ApplicationDbContext context)
        {
            _dbWriterContext = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbWriterContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbWriterContext.Set<T>().FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbWriterContext.Set<T>().AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbWriterContext.Set<T>().Attach(entity);
            _dbWriterContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbWriterContext.Set<T>().Remove(entity);
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
        {
            return _dbWriterContext.Set<T>().Where(predicate);
        }
    }
}