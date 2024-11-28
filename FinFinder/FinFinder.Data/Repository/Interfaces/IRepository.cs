using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FinFinder.Data.Repository.Interfaces
{
    public interface IRepository<TType, TId>
    {
        TType GetById(TId id);
        Task<TType> GetByIdAsync(TId id);

        IEnumerable<TType> GetAll();
        Task<IEnumerable<TType>> GetAllAsync();

        IQueryable<TType> GetAllAttached();
       

        void Add(TType entity);
        Task AddAsync(TType entity);

        bool Delete(TId id);
        Task<bool> DeleteAsync(TId id);

      

        bool Update(TType entity);
        Task<bool> UpdateAsync(TType entity);

        IEnumerable<TType> Find(Expression<Func<TType, bool>> predicate);
        Task<IEnumerable<TType>> FindAsync(Expression<Func<TType, bool>> predicate);

        IEnumerable<TType> GetPaged(int pageIndex, int pageSize);
        Task<IEnumerable<TType>> GetPagedAsync(int pageIndex, int pageSize);

        void AddRange(IEnumerable<TType> entities);
        Task AddRangeAsync(IEnumerable<TType> entities);

        

        bool Exists(TId id);
        Task<bool> ExistsAsync(TId id);

        int Count();
        Task<int> CountAsync();

        TType GetByIdIncluding(TId id, params Expression<Func<TType, object>>[] includeProperties);
        Task<TType> GetByIdIncludingAsync(TId id, params Expression<Func<TType, object>>[] includeProperties);
    }
}
