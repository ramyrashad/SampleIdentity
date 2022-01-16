﻿namespace SampleIdentity.Core.Common.Specifications
{
    public static class ISpecificationExtensions
    {
        public static bool IsSatisfiedBy<T>(this ISpecification<T> specification, T entity)
        {
            return specification.IsSatisifiedBy().Compile()(entity);
        }

        public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return Specification.And(left, right);
        }

        public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return Specification.Or(left, right);
        }

        public static ISpecification<T> Not<T>(this ISpecification<T> left, ISpecification<T> right)
        {
            return Specification.Not(right);
        }
    }
}