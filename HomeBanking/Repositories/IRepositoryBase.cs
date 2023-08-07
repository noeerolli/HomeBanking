using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace HomeBanking.Repositories
{
    public interface IRepositoryBase<T> //base para todos los repositorios   -  <T> dato de tipogenerico recibe cierto tipo de dato, no deinimos cual
    {
          
        IQueryable<T> FindAll();     
        IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);  //permite agregar otra query a la query q ya realizamos
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression); //agregamos una condición a la busqueda
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}

