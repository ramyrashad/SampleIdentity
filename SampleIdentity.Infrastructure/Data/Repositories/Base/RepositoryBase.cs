using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using SampleIdentity.Core.Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using SampleIdentity.Core.Repositories.Base;
using SampleIdentity.Infrastructure.Data.Context;

namespace SampleIdentity.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public RepositoryBase(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Implementation

        public virtual void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
        }

        public virtual void Update(T entity)
        {
            try
            {
                _dbContext.Set<T>().Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception)
            {

            }
        }

        public virtual bool Attach(T entity)
        {
            try
            {
                _dbContext.Set<T>().Attach(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual void DeleteList(IEnumerable<T> entities)
        {
            foreach (var obj in entities)
                _dbContext.Set<T>().Remove(obj);
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return _dbContext.Set<T>().Where(where).FirstOrDefault();
        }

        public virtual async Task<T> GetSingleOrDefaultAsync(ISpecification<T> spec)
        {
            return await BuildQueryWithSpecifications(spec).SingleOrDefaultAsync();
        }

        public virtual async Task<T> GetFirstOrDefaultAsync(ISpecification<T> spec,bool asNoTracking = false)
        {
            if (asNoTracking == false)
                return await BuildQueryWithSpecifications(spec).FirstOrDefaultAsync();
            else
                return await BuildQueryWithSpecifications(spec).AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetFirstOrDefaultAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> sortSelector = null, bool isDescending = false)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            query = isDescending ? query.OrderByDescending(sortSelector) : query.OrderBy(sortSelector);
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

            return await query.ToListAsync();
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

        public virtual async Task<Tkey> MaxAsync<Tkey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            return await query.MaxAsync(keySelector);
        }

        public virtual async Task<decimal> SumAsync(ISpecification<T> spec, Expression<Func<T, decimal>> keySelector)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            return await query.SumAsync(keySelector);
        }

        public virtual async Task<int> SumAsync(ISpecification<T> spec, Expression<Func<T, int>> keySelector)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            return await query.SumAsync(keySelector);
        }

        public virtual async Task<IEnumerable<Tkey>> Select<Tkey, OKey>(ISpecification<T> spec, Expression<Func<T, Tkey>> keySelector, Expression<Func<T, OKey>> orderSelector)
        {
            IQueryable<T> query = BuildQueryWithSpecifications(spec);

            return await query.OrderBy(orderSelector).Select(keySelector).ToListAsync();
        }

        #endregion

        #region Private Methods

        protected IQueryable<T> BuildQueryWithSpecifications(ISpecification<T> spec)
        {
            _dbContext.ChangeTracker.QueryTrackingBehavior = spec.IsTrackingEnabled ?
                QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;

            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_dbContext.Set<T>().AsQueryable(), (current, include) => current.Include(include));

            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

            if (spec.IsDeletedIncluded)
                secondaryResult = secondaryResult.IgnoreQueryFilters();

            if (spec.SingleQueryEnabled)
                secondaryResult = secondaryResult.AsSingleQuery();

            if (spec.IsTrackingEnabled == false)
                secondaryResult = secondaryResult.AsNoTracking();

            return secondaryResult.Where(spec.IsSatisifiedBy()).AsQueryable();
        }

        private IQueryable<R> BuildQueryWithSpecifications<R>(ISpecification<R> spec) where R : class
        {
            _dbContext.ChangeTracker.QueryTrackingBehavior = spec.IsTrackingEnabled ?
                QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;

            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_dbContext.Set<R>().AsQueryable(), (current, include) => current.Include(include));

            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

            if (spec.IsDeletedIncluded)
                secondaryResult = secondaryResult.IgnoreQueryFilters();

            return secondaryResult.Where(spec.IsSatisifiedBy()).AsQueryable();
        }

        #endregion

    }
}
