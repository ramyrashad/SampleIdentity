using System;
using System.Linq.Expressions;

namespace SampleIdentity.Core.Common.Specifications
{
    internal sealed class AndSpecification<T> : BaseSpecification<T>
    {
        private readonly ISpecification<T> left;
        private readonly ISpecification<T> right;

        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
        {
            this.left = left;
            this.right = right;

            foreach (var include in this.left.Includes)
                AddInclude(include);

            foreach (var include in this.left.IncludeStrings)
                AddInclude(include);


            foreach (var include in this.right.Includes)
                AddInclude(include);

            foreach (var include in this.right.IncludeStrings)
                AddInclude(include);

            if (left.IsDeletedIncluded)
                IncludeDeleted();

            if (right.IsDeletedIncluded)
                IncludeDeleted();

            if (left.SingleQueryEnabled)
                EnableSingleQuery();

            if (right.SingleQueryEnabled)
                EnableSingleQuery();

            if (left.IsTrackingEnabled == false)
                DisableTracking();

            if (right.IsTrackingEnabled == false)
                DisableTracking();
        }

        public override Expression<Func<T, bool>> IsSatisifiedBy()
        {

            var leftExpression = left.IsSatisifiedBy();
            var rightExpression = right.IsSatisifiedBy();

            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(leftExpression.Parameters[0], parameter);
            var leftPar = leftVisitor.Visit(leftExpression.Body);

            var rightVisitor = new ReplaceExpressionVisitor(rightExpression.Parameters[0], parameter);
            var rightPar = rightVisitor.Visit(rightExpression.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(leftPar, rightPar), parameter);
        }
    }
}
