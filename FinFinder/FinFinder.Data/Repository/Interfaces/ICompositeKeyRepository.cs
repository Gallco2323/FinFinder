using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Repository.Interfaces
{
    public interface ICompositeKeyRepository<TEntity, TKey1, TKey2>
    where TEntity : class
    {
        TEntity GetByCompositeKey(TKey1 key1, TKey2 key2);
        Task<TEntity> GetByCompositeKeyAsync(TKey1 key1, TKey2 key2);
        bool DeleteByCompositeKey(TKey1 key1, TKey2 key2);
        Task<bool> DeleteByCompositeKeyAsync(TKey1 key1, TKey2 key2);
        IQueryable<TEntity> GetAllAttached();

        void Add(TEntity entity); // Add method
        Task AddAsync(TEntity entity);
    }
}
