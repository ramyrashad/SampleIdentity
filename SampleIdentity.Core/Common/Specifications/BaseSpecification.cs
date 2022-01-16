using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SampleIdentity.Core.Common.Specifications
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        public List<string> IncludeStrings { get; } = new List<string>();

        public bool IsDeletedIncluded { get; private set; } = false;

        public bool IsTrackingEnabled { get; private set; } = true;

        public bool SingleQueryEnabled { get; private set; } = false;

        public abstract Expression<Func<T, bool>> IsSatisifiedBy();

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public virtual void IncludeDeleted()
        {
            IsDeletedIncluded = true;
        }

        public virtual void DisableTracking()
        {
            IsTrackingEnabled = false;
        }

        public virtual void ClearIncludes()
        {
            this.IncludeStrings.Clear();
            this.Includes.Clear();
        }

        public virtual void EnableSingleQuery()
        {
            this.SingleQueryEnabled = true;
        }

        protected class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return base.VisitParameter(_parameter);
            }

            internal ParameterReplacer(ParameterExpression parameter)
            {
                _parameter = parameter;
            }
        }

        protected class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                    return _newValue;

                return base.Visit(node);
            }
        }
    }
}
