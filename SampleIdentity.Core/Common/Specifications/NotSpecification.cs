using System;
using System.Linq.Expressions;

namespace SampleIdentity.Core.Common.Specifications
{
    internal sealed class NotSpecification<T> : BaseSpecification<T>
    {
        private readonly ISpecification<T> specification;
        
        public NotSpecification(ISpecification<T> specification)
        {
            this.specification = specification;
        }

        public override Expression<Func<T, bool>> IsSatisifiedBy()
        {
            var expression = specification.IsSatisifiedBy();

            var parameter = expression.Parameters[0];
            var body = Expression.Not(expression.Body);

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }
    }
}
