using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RouterManagement.Logic.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        bool Any(Func<T, bool> where);
        IQueryable<T> AsQueryable();
        void Delete(T entity);
        void Delete(Func<T, bool> where);
        IEnumerable<T> Find(Func<T, bool> where);
        T First(Expression<Func<T, bool>> where);
        T FirstOrDefault(Func<T, bool> where);
        IEnumerable<T> GetAll();
        T Single(Func<T, bool> where);
        void Update(T entity);
        void Dispose();
    }
}
