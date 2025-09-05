using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {

        T Get(string id);
    
        void Insert(T entity);
        void Update(T entity);
        void Delete(string id);
        T Delete(T entity);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(string id);
        IQueryable<T> GetAllAsyncAsQuery();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);

        Task<List<object>> FindAll(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> Object);
        Task<List<object>> FindAll(Expression<Func<T, object>> Object);
        Task<List<T>> FindAll(Expression<Func<T, bool>> predicate);
        Task<List<string>> FindAll(Expression<Func<T, bool>> predicate, Expression<Func<T, string>> Object);

        T Find(Expression<Func<T, bool>> predicate);

        Task<object> Mapping(Expression<Func<T, object>> Object);
        Task<object> Find(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> Object);

        Task<bool> Any(Expression<Func<T, bool>> predicate);

    }
}
