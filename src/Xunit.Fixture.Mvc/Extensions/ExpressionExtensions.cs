using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Xunit.Fixture.Mvc.Extensions
{
    internal static class ExpressionExtensions
    {
        public static PropertyInfo GetProperty<TModel, TProperty>(this Expression<Func<TModel, TProperty>> property) =>
            property.GetMember() as PropertyInfo ?? throw new ArgumentException("Not a property expression: " + property, nameof(property));

        public static MemberInfo GetMember<TModel, TProperty>(this Expression<Func<TModel, TProperty>> property)
        {
            switch (property.Body)
            {
                case MemberExpression member:
                    return member.Member;

                case UnaryExpression unary when unary.Operand is MemberExpression operand:
                    return operand.Member;

                default:
                    throw new ArgumentException("Not a member or unary expression: " + property, nameof(property));
            }
        }
    }
}
