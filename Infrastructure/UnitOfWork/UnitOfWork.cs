
using Core.Interfaces;

namespace Infrastructure.UnitOfWork
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
                return entity ?? (entity = new Repository.GenericRepository<T>(context));
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
