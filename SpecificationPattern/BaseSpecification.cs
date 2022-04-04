using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using SpecificationPattern.Interfaces;
using SpecificationPattern.Utils.Extensions;

namespace SpecificationPattern;

public class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; set; }
    public List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includes { get; private init; } = new();
    public List<string> IncludeStrings { get; private init; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    protected virtual void AddInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeExpression) => Includes.Add(includeExpression);

    protected virtual void AddInclude(string includeString) => IncludeStrings.Add(includeString);
    
    protected virtual void AddOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;

    protected virtual void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression) => OrderByDescending = orderByDescExpression;
    
    public static BaseSpecification<T> operator &(BaseSpecification<T> spec1, BaseSpecification<T> spec2)
    {
        var includes = new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>>(spec1.Includes);
        includes.AddRange(spec2.Includes);
        var includeString = new List<string>(spec1.IncludeStrings);
        includeString.AddRange(spec2.IncludeStrings);
        return new BaseSpecification<T>()
        {
            Criteria = spec1.Criteria?.And(spec2.Criteria),
            Includes = includes,
            IncludeStrings = includeString,
            OrderBy = spec2.OrderBy,
            OrderByDescending = spec2.OrderByDescending
        };
    }
    
    public static BaseSpecification<T> operator |(BaseSpecification<T> spec1, BaseSpecification<T> spec2)
    {
        var includes = new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>>(spec1.Includes);
        includes.AddRange(spec2.Includes);
        var includeString = new List<string>(spec1.IncludeStrings);
        includeString.AddRange(spec2.IncludeStrings);
        return new BaseSpecification<T>()
        {
            Criteria = spec1.Criteria?.Or(spec2.Criteria),
            Includes = includes,
            IncludeStrings = includeString,
            OrderBy = spec2.OrderBy,
            OrderByDescending = spec2.OrderByDescending
        };
    }
    
    public static BaseSpecification<T> operator !(BaseSpecification<T> spec)
    {
        Expression<Func<T, bool>>? Inverse(Expression<Func<T, bool>>? expression) => expression is null
            ? null
            : Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);
        
        return new BaseSpecification<T>()
        {
            Criteria = Inverse(spec.Criteria),
            Includes = spec.Includes,
            IncludeStrings = spec.IncludeStrings,
            OrderBy = spec.OrderBy,
            OrderByDescending = spec.OrderByDescending
        };
    }

    public static bool operator false(BaseSpecification<T> spec1) => false;

    public static bool operator true(BaseSpecification<T> spec1) => true;
}