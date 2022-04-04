using Microsoft.EntityFrameworkCore;
using SpecificationPattern.Interfaces;

namespace SpecificationPattern.Utils.Extensions;

public static class QuerySpecificationExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class
    {
        var queryableResultWithIncludes = spec.Includes
            .Aggregate(query, (current, include) => include(current));

        var secondaryResult = spec.IncludeStrings
            .Aggregate(queryableResultWithIncludes, (current, include) => current.Include(include));

        var criteriaResult = spec.Criteria is null ? secondaryResult : secondaryResult.Where(spec.Criteria);
        var orderByResult = spec.OrderBy is null ? criteriaResult : criteriaResult.OrderBy(spec.OrderBy);
        return spec.OrderByDescending is null ? orderByResult : orderByResult.OrderByDescending(spec.OrderByDescending);
    }
}