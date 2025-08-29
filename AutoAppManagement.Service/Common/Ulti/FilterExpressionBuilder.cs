using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoAppManagement.Models.Common;

namespace AutoAppManagement.Service.Common.Ulti
{
    public static class FilterExpressionBuilder
    {
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static readonly MethodInfo StartsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static readonly MethodInfo EndsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        public static Expression<Func<T, bool>> GetExpression<T>(IList<FilterCondition> filters)
        {
            if (filters == null || !filters.Any())
                return null;

            var param = Expression.Parameter(typeof(T), "t");
            var body = BuildExpression(param, filters, LogicalOperator.AND);
            return body == null ? null : Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression BuildExpression(ParameterExpression param, IEnumerable<FilterCondition> filters, LogicalOperator op)
        {
            var expressions = new List<Expression>();
            foreach (var filter in filters)
            {
                var mainExp = CreateSingleExpression(param, filter);
                if (mainExp == null) continue;

                var orExp = filter.ors != null && filter.ors.Any() 
                    ? BuildExpression(param, filter.ors, LogicalOperator.OR) 
                    : null;

                expressions.Add(orExp == null ? mainExp : Expression.Or(mainExp, orExp));
            }

            return expressions.Any() 
                ? expressions.Aggregate((left, right) => op == LogicalOperator.OR ? Expression.Or(left, right) : Expression.And(left, right)) 
                : null;
        }

        private static Expression CreateSingleExpression(ParameterExpression param, FilterCondition filter)
        {
            try
            {
                var member = Expression.Property(param, filter.field);
                var propertyType = ((PropertyInfo)member.Member).PropertyType;
                var value = ConvertValue(filter.value, propertyType);
                var constant = Expression.Constant(value, propertyType);

                return filter.op switch
                {
                    FilterOperator.Equals => Expression.Equal(member, constant),
                    FilterOperator.NotEquals => Expression.NotEqual(member, constant),
                    FilterOperator.Contains => Expression.Call(member, ContainsMethod, Expression.Constant(value as string)),
                    FilterOperator.StartsWith => Expression.Call(member, StartsWithMethod, Expression.Constant(value as string)),
                    FilterOperator.EndsWith => Expression.Call(member, EndsWithMethod, Expression.Constant(value as string)),
                    FilterOperator.GreaterThan or FilterOperator.GreaterThan_Date => Expression.GreaterThan(member, constant),
                    FilterOperator.GreaterThanOrEqual or FilterOperator.GreaterThanOrEqual_Date => Expression.GreaterThanOrEqual(member, constant),
                    FilterOperator.LessThan or FilterOperator.LessThan_Date => Expression.LessThan(member, constant),
                    FilterOperator.LessThanOrEqual or FilterOperator.LessThanOrEqual_Date => Expression.LessThanOrEqual(member, constant),
                    _ => null,
                };
            }
            catch (Exception)
            {
                // Ignore invalid filters (e.g., wrong field name)
                return null;
            }
        }

        private static object ConvertValue(string value, Type type)
        {
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return DateTime.Parse(value).ToUniversalTime();
            }
            if (type.IsEnum)
            {
                return Enum.ToObject(type, int.Parse(value));
            }
            if (type == typeof(Guid))
            {
                return Guid.Parse(value);
            }
            return Convert.ChangeType(value, Nullable.GetUnderlyingType(type) ?? type);
        }
    }
}
