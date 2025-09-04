using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Rrpository
{
    public class GenericRepository<T> : Interfaces.IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private readonly DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public void Delete(object id)
        {
            var entity = Get(id);
            if (entity == null)
                return;
            dbSet.Remove(entity);
        }

        public T Get(object id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.AsNoTracking().ToList();
            
        }

        public object GetElement(object id)
        {
            return dbSet.Find(id);
        }

        public void Insert(T entity)
        {
            dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            context.Entry(entity).State = EntityState.Modified;
        }
    }
}
