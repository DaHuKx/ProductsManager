namespace ProductsManager.Infrastructure.Repositories.Interfaces
{
    public interface IRepositoryBase<T>
    {
        Task<T?> AddAsync(T value);
        T? Add(T value);
        Task<IEnumerable<T>?> AddRangeAsync(IEnumerable<T> values);
        Task<List<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        T? Remove(int id);
    }
}
