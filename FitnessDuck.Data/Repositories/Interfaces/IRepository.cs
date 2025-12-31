namespace FitnessDuck.Data.Repositories.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?>  AddAsync(T entity, bool autoSave = true);
    Task<T?> Update(T entity, bool autoSave = true);
    Task Remove(T entity, bool autoSave = true);
    Task Remove(Guid id, bool autoSave = true);
    Task<T?> AddOrUpdateAsync(T entity, bool autoSave = true);
    Task SaveChangesAsync();
}