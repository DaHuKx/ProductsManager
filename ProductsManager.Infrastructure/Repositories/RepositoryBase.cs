using Microsoft.EntityFrameworkCore;
using ProductsManager.Domain;
using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseDbEntity
    {
        protected readonly ProductsManagerDb Context;

        public RepositoryBase(ProductsManagerDb productsManager)
        {
            Context = productsManager;
        }

        public virtual async Task<T?> GetAsync(int id)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(v => v.Id == id);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public virtual async Task<T?> AddAsync(T value)
        {
            try
            {
                await Context.Set<T>().AddAsync(value);
                await Context.SaveChangesAsync();

                return value;
            }
            catch
            {
                return null;
            }
        }

        public virtual T? Add(T value)
        {
            try
            {
                Context.Set<T>().Add(value);
                Context.SaveChanges();

                return value;
            }
            catch
            {
                return null;
            }
        }

        public virtual async Task<IEnumerable<T>?> AddRangeAsync(IEnumerable<T> values)
        {
            try
            {
                await Context.Set<T>().AddRangeAsync(values);
                await Context.SaveChangesAsync();

                return values;
            }
            catch
            {
                return null;
            }
        }

        public virtual T? Remove(int id)
        {
            var value = Context.Set<T>().FirstOrDefault(v => v.Id == id);

            if (value == null) return null;

            try
            {
                Context.Set<T>().Remove(value);
                Context.SaveChanges();

                return value;
            }
            catch
            {
                return null;
            }
        }
    }
}
