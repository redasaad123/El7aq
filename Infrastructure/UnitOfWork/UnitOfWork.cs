
using Core.Interfaces;
using Infrastructure;

namespace Core.UnitOfWork
{
    public class UnitOfWork<T> : IUnitOfWork<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private  IGenericRepository<T> entity;


        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
        }
        public IGenericRepository<T> Entity
        {
            get
            {
                return entity ?? (entity = new Rrpository.GenericRepository<T>(context));
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
