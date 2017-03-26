using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RouterManagement.Logic.Repositories.Interfaces
{
    public interface IGenericRepository<T>
        where T : class
    {
        void Add(T entity);

        bool Any(Expression<Func<T, bool>> where);

        IQueryable<T> AsQueryable();

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> where);

        IEnumerable<T> Find(Expression<Func<T, bool>> where);

        T First(Expression<Func<T, bool>> where);

        T FirstOrDefault(Expression<Func<T, bool>> where);

        IEnumerable<T> GetAll();

        T Single(Expression<Func<T, bool>> where);

        void Update(T entity);
        void Dispose();
    }
}