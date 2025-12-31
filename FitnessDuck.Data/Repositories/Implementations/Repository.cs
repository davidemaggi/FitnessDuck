using FitnessDuck.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessDuck.Data.Repositories.Implementations;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly FitnessDuckDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(FitnessDuckDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task<T?> AddAsync(T entity, bool autoSave = true)
    {
        var res=await _dbSet.AddAsync(entity);
        

        if (!autoSave) return null;
        
        await SaveChangesAsync();
        return entity;

    }
   

    public async Task<T?> Update(T entity, bool autoSave = true){
        var res=  _dbSet.Update(entity);
        

        if (!autoSave) return null;
        
        await SaveChangesAsync();
        return entity;

    }

    public async Task Remove(T entity, bool autoSave = true)
    {
        _dbSet.Remove(entity);
        if (autoSave) 
            await SaveChangesAsync();
    }

    public async Task Remove(Guid id, bool autoSave = true)
    {
        var toRemove = _dbSet.Find(id);
        if (toRemove == null)
            return;
         _dbSet.Remove(toRemove);
         if (autoSave) 
             await SaveChangesAsync();
    }


    public async Task<T?> AddOrUpdateAsync(T entity, bool autoSave = true)
    {
        var keyProperty = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties[0];
        if (keyProperty == null)
            throw new InvalidOperationException($"Entity '{typeof(T).Name}' does not have a primary key defined.");

        var keyValue = keyProperty.PropertyInfo?.GetValue(entity);
        if (keyValue == null || keyValue.Equals(GetDefault(keyProperty.ClrType)))
        {
            // Key not set or default, treat as new entity
            await AddAsync(entity);
        }
        else
        {
            var existingEntity = await _dbSet.FindAsync(keyValue);
            if (existingEntity == null)
            {
                await AddAsync(entity);
            }
            else
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
        }

        if (!autoSave) return null;
        await SaveChangesAsync();
        return entity;



    }

    private static object? GetDefault(Type type)
    {
        // Handle Nullable<T>
        if (type.IsValueType)
            return Activator.CreateInstance(type);
        return null;
    }

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}