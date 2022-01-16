using Microsoft.EntityFrameworkCore;
using SampleIdentity.Core.Common.Specifications;
using SampleIdentity.Core.Repositories.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SampleIdentity.Infrastructure.Data.Context;

namespace SampleIdentity.Infrastructure.Repositories
{
    public class RepositoryBaseReadOnly<T> : IRepositoryReadonly<T> where T : class
    {
        #region Properties
        protected ApplicationDbContext _dbContext;

        #endregion

        public RepositoryBaseReadOnly(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Implementation
        public virtual T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual T GetById(Guid id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual T GetById(string id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Where(where).FirstOrDefault();
        }

        public virtual async Task<T> GetSingleOrDefaultAsync(ISpecification<T> spec)
        {
            return await BuildQueryWithSpecifications(spec).SingleOrDefaultAsync();
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(ISpecification<T> spec)
        {
            return await BuildQueryWithSpecifications(spec).FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetFirstOrDefaultAsync<Tkey>(ISpecification<T> spec,
          Expression<Func<T, Tkey>> keySelector = null,
          bool isDescending = false)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            query = isDescending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ListAllAsync(ISpecification<T> spec)
        {
            return await BuildQueryWithSpecifications(spec).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec,
            Expression<Func<T, Tkey>> keySelector = null,
            bool isDescending = false)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            query = isDescending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector);
            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ListAllAsync<Tkey>(
            ISpecification<T> spec,
            int pageIndex, int pageSize,
            Expression<Func<T, Tkey>> keySelector,
            bool isDescending)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            query = isDescending ?
                query.OrderByDescending(keySelector).Skip(pageIndex * pageSize).Take(pageSize) :
                query.OrderBy(keySelector).Skip(pageIndex * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ListAllAsync<Tkey>(ISpecification<T> spec,
            Expression<Func<T, Tkey>> keySelector, Sorting sorting)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            query = sorting.SortDirection == Sorting.SortDirectionType.Descending ?
                query.OrderByDescending(keySelector).Skip(sorting.PageIndex * sorting.PageSize).Take(sorting.PageSize) :
                query.OrderBy(keySelector).Skip(sorting.PageIndex * sorting.PageSize).Take(sorting.PageSize);

            //var str = query.ToSql();

            return await query.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> DistinctAsync<Tkey>(ISpecification<T> spec,
        Expression<Func<T, Tkey>> keySelector, Sorting sorting)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            query = sorting.SortDirection == Sorting.SortDirectionType.Descending ?
                query.OrderByDescending(keySelector).Skip(sorting.PageIndex * sorting.PageSize).Take(sorting.PageSize) :
                query.OrderBy(keySelector).Skip(sorting.PageIndex * sorting.PageSize).Take(sorting.PageSize);

            return await query.Distinct().ToListAsync();
        }

        public virtual async Task<bool> AnyAsync(ISpecification<T> spec)
        {
            return await BuildQueryWithSpecifications(spec).AnyAsync();
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public virtual async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await BuildQueryWithSpecifications(spec).CountAsync();
        }

        public virtual async Task<double?> SumAsync(ISpecification<T> spec, Expression<Func<T, double?>> keySelector)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);
            return await query.SumAsync(keySelector);
        }

        public virtual async Task<int?> SumAsync(ISpecification<T> spec, Expression<Func<T, int?>> keySelector)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);
            return await query.SumAsync(keySelector);
        }


        #endregion

        #region Private Methods

        protected IQueryable<T> BuildQueryWithSpecifications(ISpecification<T> spec)
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_dbContext.Set<T>().AsQueryable(), (current, include) => current.Include(include));

            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

            if (spec.IsDeletedIncluded)
                secondaryResult = secondaryResult.IgnoreQueryFilters();

            return secondaryResult.Where(spec.IsSatisifiedBy()).AsQueryable();
        }

        #endregion
    }
}
