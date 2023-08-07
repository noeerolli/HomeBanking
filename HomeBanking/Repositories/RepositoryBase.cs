using HomeBanking.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace HomeBanking.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected HomeBankingContext RepositoryContext { get; set; }    //creamos una propiedad con el tipo de dato HomeBContext, le damos un nombre de variable y sus getter y setter  / protected porq puede acceder la clase y sus desendientes

        public RepositoryBase(HomeBankingContext repositoryContext)   //constructor, cuando las clases que hereden sean instanciadas, voy a pasarle algo de tipo HBContext
        {
            this.RepositoryContext = repositoryContext;
        }

        public IQueryable<T> FindAll()
        {
            //return this.RepositoryContext.Set<T>().AsNoTracking();
            return this.RepositoryContext.Set<T>().AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            IQueryable<T> queryable = this.RepositoryContext.Set<T>();  //trae todo

            if (includes != null)
            {
                queryable = includes(queryable);      //aplica una query a la query que ya realizó, la extiende
            }
            return queryable.AsNoTrackingWithIdentityResolution();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.RepositoryContext.Set<T>().Where(expression).AsNoTrackingWithIdentityResolution();
        }

        public void Create(T entity)
        {
            this.RepositoryContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.RepositoryContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.RepositoryContext.Set<T>().Remove(entity);
        }

        public void SaveChanges()
        {
            this.RepositoryContext.SaveChanges();
        }
    }
}
