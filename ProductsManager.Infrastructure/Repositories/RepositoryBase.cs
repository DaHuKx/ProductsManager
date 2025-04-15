using Microsoft.EntityFrameworkCore;
using ProductsManager.Domain;
using ProductsManager.Infrastructure.DataBase;
using ProductsManager.Infrastructure.Repositories.Interfaces;

namespace ProductsManager.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseDbEntity
    {
        public virtual async Task<T?> GetAsync(int id)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                return await context.Set<T>().FirstOrDefaultAsync(v => v.Id == id);
            }
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                return await context.Set<T>().ToListAsync();
            }
        }

        public virtual async Task<T?> AddAsync(T value)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                try
                {
                    await context.Set<T>().AddAsync(value);
                    await context.SaveChangesAsync();

                    return value;
                }
                catch
                {
                    return null;
                }
            }
        }

        public virtual T? Add(T value)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                try
                {
                    context.Set<T>().Add(value);
                    context.SaveChanges();

                    return value;
                }
                catch
                {
                    return null;
                }
            }
        }

        public virtual async Task<IEnumerable<T>?> AddRangeAsync(IEnumerable<T> values)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                try
                {
                    await context.Set<T>().AddRangeAsync(values);
                    await context.SaveChangesAsync();

                    return values;
                }
                catch
                {
                    return null;
                }
            }
        }

        public virtual T? Remove(int id)
        {
            using (ProductsManagerDb context = new ProductsManagerDb())
            {
                var value = context.Set<T>().FirstOrDefault(v => v.Id == id);

                if (value == null) return null;

                try
                {
                    context.Set<T>().Remove(value);
                    context.SaveChanges();

                    return value;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
