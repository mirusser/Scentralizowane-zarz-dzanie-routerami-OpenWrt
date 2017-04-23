using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using RouterManagement.Logic.Repositories.Interfaces;
using RouterManagement.Models.Context;

namespace RouterManagement.Logic.Repositories
{
    public class GenericRepository<T, TEntity> : IGenericRepository<T>, IDisposable
        where T : class
        where TEntity : RouterManagementEntities, new()
    {
        private readonly TEntity ctx;
        private readonly DbSet<T> dbSet;

        public GenericRepository(TEntity ctx)
        {
            this.ctx = ctx;
            dbSet = ctx.Set<T>();
        }

        public void Add(T entity)
        {
            if (entity == null) return;
            dbSet.Add(entity);
        }

        public bool Any(Expression<Func<T, bool>> where)
        {
            return dbSet.Any(where);
        }

        public bool Any()
        {
            return dbSet.Any();
        }

        public IQueryable<T> AsQueryable()
        {
            return dbSet.AsQueryable();
        }

        public void Delete(T entity)
        {
            if (entity == null) return;

            ctx.Set<T>().Remove(entity);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            var itemsToDelete = dbSet.Where(where);

            dbSet.RemoveRange(itemsToDelete);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> where)
        {
            return dbSet.Where(where);
        }

        public T First(Expression<Func<T, bool>> where)
        {
            return dbSet.First(where);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> where)
        {
            return dbSet.FirstOrDefault(where);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.AsEnumerable();
        }

        public T Single(Expression<Func<T, bool>> where)
        {
            return dbSet.Single(where);
        }

        public void Update(T entity)
        {
            if (entity == null) return;
            ctx.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Dispose()
        {
            ctx?.Dispose();
        }
    }
}