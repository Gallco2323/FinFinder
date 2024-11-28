using FinFinder.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Repository
{
    public class BaseRepository<TType, TId> : IRepository<TType, TId>
        where TType : class
    {
        private readonly FinFinderDbContext _context;
        private readonly DbSet<TType> _dbSet;

        public BaseRepository(FinFinderDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TType>();
        }
        public void Add(TType entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public async Task AddAsync(TType entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void AddRange(IEnumerable<TType> entities)
        {
            _dbSet.AddRange(entities);
            _context.SaveChanges();
        }

        public async Task AddRangeAsync(IEnumerable<TType> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public bool Delete(TId id)
        {
            var entity = this.GetById(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
             _context.SaveChanges();
            return true;

        }

        public async Task<bool> DeleteAsync(TId id)
        {
           TType entity = await this.GetByIdAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool Exists(TId id)
        {
            return _dbSet.Find(id) != null;
        }

        public async Task<bool> ExistsAsync(TId id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public IEnumerable<TType> Find(Expression<Func<TType, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        public async Task<IEnumerable<TType>> FindAsync(Expression<Func<TType, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public IEnumerable<TType> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IEnumerable<TType>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<TType> GetAllAttached()
        {
            return this._dbSet.AsQueryable();
        }

       

        public TType GetById(TId id)
        {
            return _dbSet.Find(id);
        }

        public async Task<TType> GetByIdAsync(TId id)
        {
            return await _dbSet.FindAsync(id);
        }

        public TType GetByIdIncluding(TId id, params Expression<Func<TType, object>>[] includeProperties)
        {
            IQueryable<TType> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.FirstOrDefault(e => EF.Property<TId>(e, "Id").Equals(id));
        }

        public async Task<TType> GetByIdIncludingAsync(TId id, params Expression<Func<TType, object>>[] includeProperties)
        {
            IQueryable<TType> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(e => EF.Property<TId>(e, "Id").Equals(id));
        }

        public IEnumerable<TType> GetPaged(int pageIndex, int pageSize)
        {
            return _dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<IEnumerable<TType>> GetPagedAsync(int pageIndex, int pageSize)
        {
            return await _dbSet.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }

      

        public bool Update(TType entity)
        {
            try
            {
                this._dbSet.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
                this._context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
       
        }

        public async Task<bool> UpdateAsync(TType entity)
        {
            try
            {
                this._dbSet.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
              await  this._context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

       
    }
}
