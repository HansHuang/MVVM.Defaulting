using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting.Extensions
{
    public static class ExpressionExtension
    {
        public static PropertyInfo GetPropety(this LambdaExpression expression)
        {
            return expression.GetMemberExp().Member as PropertyInfo;
        }

        /// <summary>
        /// Get property name from lambda expression such as "s => s.Profile.Name" returns "Profile.Name"
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetPropetyName(this LambdaExpression expression)
        {
            var nodes = expression.GetChainNodes().Select(s => s.Member.Name).ToList();
            var result = string.Join(".", nodes);
            return result;
        }

        #region GetAllPropertyNames

        /// <summary>
        /// Interate expression to get all property names
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetAllPropertyNames(this LambdaExpression exp)
        {
            var subExps = exp.InterateAll().OfType<MemberExpression>().ToList();
            var allChainNodes = subExps.SelectMany(s => s.GetChainNodes().Except(new[] { s })).ToList();

            subExps = subExps.Except(allChainNodes).ToList();

            return subExps.Select(s => string.Join(".", s.GetChainNodes().Select(x => x.Member.Name))).ToList();
        }

        ///// <summary>
        ///// Interate expression to get all property names
        ///// </summary>
        ///// <param name="exp">Target Expression</param>
        ///// <returns>Property names from Expression</returns>
        //public static List<string> GetAllPropertyNames(this LambdaExpression exp)
        //{
        //    var subExps = exp.InterateAll().OfType<MemberExpression>();
        //    var names = subExps.Select(s => s.Member.Name).ToList();
        //    return names;
        //}

        /// <summary>
        /// Interate Expression(Recursion)
        /// </summary>
        /// <param name="exp">Target Expression</param>
        /// <returns>All Expressions include self</returns>
        public static List<Expression> InterateAll(this Expression exp)
        {
            var result = new List<Expression> { exp };

            var type = exp.GetType();
            if (type.IsGenericType) type = type.BaseType;
            var propInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var childExps = new List<Expression>();
            foreach (var pi in propInfos)
            {
                if (typeof(Expression).IsAssignableFrom(pi.PropertyType))
                {
                    childExps.Add((Expression)pi.GetValue(exp, null));
                }
                else if (typeof(IEnumerable<Expression>).IsAssignableFrom(pi.PropertyType))
                {
                    childExps.AddRange((IEnumerable<Expression>)pi.GetValue(exp, null));
                }
            }
            childExps.Where(s => s != null).ForEach(s => result.AddRange(s.InterateAll()));
            return result;
        }
        #endregion

        public static List<MemberExpression> GetChainNodes(this LambdaExpression property)
        {
            var member = property.GetMemberExp();
            return member.GetChainNodes();
        }

        public static List<MemberExpression> GetChainNodes(this MemberExpression exp)
        {
            var list = new List<MemberExpression>();
            var member = exp;
            while (member != null)
            {
                list.Add(member);
                member = member.Expression as MemberExpression;
            }
            list.Reverse();
            return list;
        }

        public static MemberExpression GetMemberExp(this LambdaExpression property)
        {
            var memberExp = property.Body as MemberExpression;
            if (memberExp == null)
            {
                var unary = property.Body as UnaryExpression;
                if (unary == null) return null;

                memberExp = unary.Operand as MemberExpression;
            }
            return memberExp;
        }

        public static Func<object> GetGetter(this MemberExpression memberExp)
        {
            var cvt = Expression.Convert(memberExp, typeof(object));
            var getterExp = Expression.Lambda<Func<object>>(cvt);
            return getterExp.Compile();
        }

        /// <summary>
        /// Get property Setter Mehod by build lambda expression
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyExp"></param>
        /// <returns></returns>
        public static Action<TObject, TProperty> GetPropertySetter<TObject, TProperty>(this Expression<Func<TObject, TProperty>> propertyExp)
        {
            var getterExp = propertyExp.GetMemberExp();
            var objParam = (ParameterExpression)getterExp.Expression;
            var propParam = Expression.Parameter(typeof(TProperty), getterExp.Member.Name);

            var rightExp = typeof(TProperty) == getterExp.Type
                ? (Expression)propParam
                : Expression.Convert(propParam, getterExp.Type);

            Action<TObject, TProperty> result = Expression.Lambda<Action<TObject, TProperty>>
            (
                Expression.Assign(getterExp, rightExp), objParam, propParam
            ).Compile();

            return result;
        }

        /// <summary>
        ///  && operation for two Predicates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Predicate<T> And<T>(this Predicate<T> left, Predicate<T> right)
        {
            return delegate(T item)
            {
                return left(item) && right(item);
            };
        }

        /// <summary>
        ///  || operation for two Predicates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Predicate<T> Or<T>(this Predicate<T> left, Predicate<T> right)
        {
            return delegate(T item)
            {
                return left(item) || right(item);
            };
        }

    }
}
