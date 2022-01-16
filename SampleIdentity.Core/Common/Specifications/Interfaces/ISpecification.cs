using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SampleIdentity.Core.Common.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> IsSatisifiedBy();

        List<Expression<Func<T, object>>> Includes { get; }

        List<string> IncludeStrings { get; }

        bool IsDeletedIncluded { get; }

        bool IsTrackingEnabled { get; }

        bool SingleQueryEnabled { get; }

        void DisableTracking();

        void ClearIncludes();

        void IncludeDeleted();

        void EnableSingleQuery();
    }
}