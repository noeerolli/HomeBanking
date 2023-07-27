using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace HomeBanking.Repositories
{
    public interface IRepositoryBase<T> //generico recibe cierto tipo de dato, no fecinimos cual
    {
          
        IQueryable<T> FindAll();
        IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}

