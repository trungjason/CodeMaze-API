using Contacts.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext RepositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            RepositoryContext = repositoryContext;
        }

        public void Create(T entity)
        {
            RepositoryContext.Add(entity);
        }

        public void Delete(T entity)
        {
            RepositoryContext.Remove(entity);
        }


        public void Update(T entity)
        {
            RepositoryContext.Update(entity);
        }

        public IQueryable<T> FindAll(bool trackChanges)
        {
            return trackChanges ? RepositoryContext.Set<T>() : RepositoryContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(System.Linq.Expressions.Expression<Func<T, bool>> expression, bool trackChanges)
        {
            IQueryable<T> findByConditionResult = RepositoryContext.Set<T>().Where(expression);

            return trackChanges ? findByConditionResult : findByConditionResult.AsNoTracking();
        }
    }
}
