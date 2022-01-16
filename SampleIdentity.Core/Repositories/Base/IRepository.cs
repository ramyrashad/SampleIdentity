using SampleIdentity.Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Repositories.Base
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        Task<bool> AnyAsync(ISpecification<T> spec);
        Task<int> CountAsync();
        Task<int> CountAsync(ISpecification<T> spec);
        void Delete(T entity);
        void DeleteList(IEnumerable<T> entities);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(string id);
        Task<T> GetByIdAsync(int id);
        Task<T> GetFirstOrDefaultAsync(ISpecification<T> spec, bool asNoTracking = false);
        Task<T> GetFirstOrDefaultAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> sortSelector = null, bool isDescending = false);
        Task<T> GetSingleOrDefaultAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> ListAllAsync();
        Task<IEnumerable<T>> ListAllAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector = null, bool isDescending = false);
        Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector, Sorting sorting);
        Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec, int pageIndex, int pageSize, Expression<Func<T, Tkey>> keySelector, bool isDescending);
        void Update(T entity);
        Task<Tkey> MaxAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector);
        Task<decimal> SumAsync(ISpecification<T> spec, Expression<Func<T, decimal>> keySelector);
        Task<int> SumAsync(ISpecification<T> spec, Expression<Func<T, int>> keySelector);
        Task<IEnumerable<Tkey>> Select<Tkey, OKey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector, Expression<Func<T, OKey>> orderSelector);
        bool Attach(T entity);
    }
}
