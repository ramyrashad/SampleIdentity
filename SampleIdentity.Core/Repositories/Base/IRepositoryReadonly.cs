using SampleIdentity.Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SampleIdentity.Core.Repositories.Interfaces.Base
{ 
    public interface IRepositoryReadonly<T> where T : class
    {
        Task<bool> AnyAsync(ISpecification<T> spec);
        Task<int> CountAsync();
        Task<int> CountAsync(ISpecification<T> spec);
        T Get(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync(); 
        T GetById(Guid id);
        T GetById(int id);
        T GetById(string id);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id);
        Task<T> GetFirstOrDefaultAsync(ISpecification<T> spec);
        Task<T> GetFirstOrDefaultAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector = null, bool isDescending = false);
        Task<T> GetSingleOrDefaultAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> ListAllAsync();
        Task<IEnumerable<T>> ListAllAsync(ISpecification<T> spec);
        Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector = null, bool isDescending = false);
        Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec, int pageIndex, int pageSize, Expression<Func<T, Tkey>> keySelector, bool isDescending);
        Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector, Sorting sorting);
        Task<double?> SumAsync(ISpecification<T> spec, Expression<Func<T, double?>> keySelector);
        Task<int?> SumAsync(ISpecification<T> spec, Expression<Func<T, int?>> keySelector);
    }
}
