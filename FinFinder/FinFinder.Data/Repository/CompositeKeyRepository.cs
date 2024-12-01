using FinFinder.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Repository
{
    public class CompositeKeyRepository<TEntity, TKey1, TKey2> : ICompositeKeyRepository<TEntity, TKey1, TKey2> 
    where TEntity : class
    {
        private readonly FinFinderDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public CompositeKeyRepository(FinFinderDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public TEntity GetByCompositeKey(TKey1 key1, TKey2 key2)
        {
            return _dbSet.Find(key1, key2);
        }

        public async Task<TEntity> GetByCompositeKeyAsync(TKey1 key1, TKey2 key2)
        {
            return await _dbSet.FindAsync(key1, key2);
        }

        public bool DeleteByCompositeKey(TKey1 key1, TKey2 key2)
        {
            var entity = _dbSet.Find(key1, key2);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> DeleteByCompositeKeyAsync(TKey1 key1, TKey2 key2)
        {
            var entity = await _dbSet.FindAsync(key1, key2);
            if (entity == null)
            {
                return false;
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public IQueryable<TEntity> GetAllAttached()
        {
            return _dbSet.AsQueryable();
        }

        public void Add(TEntity entity)
        {

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
    }
}
