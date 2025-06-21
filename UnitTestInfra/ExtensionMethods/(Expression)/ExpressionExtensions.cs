#pragma warning disable IDE0130

using Infra.ExtensionMethods;
using System.Linq.Expressions;

namespace Infra.ExtensionMethods
{
    #region 操作Expression的扩展方法

    /*
    这2个类来自,然后进行了稍微调整了,增加了null值判断
    https://docs.microsoft.com/zh-cn/archive/blogs/meek/linq-to-entities-combining-predicates

     Represents the parameter rebinder used for rebinding the parameters
     for the given expressions. This is part of the solution which solves
     the expression parameter problem when going to Entity Framework.
     For more information about this solution please refer to http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.  (这个地址失效了, 估计就是微软的docs那个)
    */

    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> Where<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return WhereIf(first, true, second);
        }

        public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> first, bool condition, Expression<Func<T, bool>> second)
        {
            if (condition)
            {
                if (first == null)
                {
                    if (second == null)
                    {
                        return first;
                    }
                    else
                    {
                        return second;
                    }
                }
                else
                {
                    return first.And(second);
                }
            }
            else
            {
                return first;
            }
        }

        /// <summary>
        /// WhereIf的 if - else  版本
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <param name="condition"></param>
        /// <param name="predicate_conditionTrue"></param>
        /// <param name="predicate_conditionFalse"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> exp, bool condition,
                                Expression<Func<T, bool>> predicate_conditionTrue,
                                Expression<Func<T, bool>> predicate_conditionFalse)
        {
            if (exp == null)
            {
                if (condition)
                {
                    return predicate_conditionTrue;
                }
                else
                {
                    return predicate_conditionFalse;
                }
            }
            else
            {
                if (condition)
                {
                    return exp.And(predicate_conditionTrue);
                }
                else
                {
                    return exp.And(predicate_conditionFalse);
                }
            }
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
            {
                if (second == null)
                {
                    return first;
                }
                else
                {
                    return second;
                }
            }
            else
            {
                return first.Compose(second, Expression.AndAlso);
            }
        }

        public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> first, bool condition, Func<Expression<Func<T, bool>>> second)
        {
            if (condition)
            {
                if (first == null)
                {
                    if (second == null)
                    {
                        return first;
                    }
                    else
                    {
                        return second();
                    }
                }
                else
                {
                    return first.Compose(second(), Expression.AndAlso);
                }
            }
            else
            {
                return first;
            }
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            if (first == null)
            {
                if (second == null)
                {
                    return first;
                }
                else
                {
                    return second;
                }
            }
            else
            {
                return first.Compose(second, Expression.OrElse);
            }
        }

        public static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> first, bool condition, Expression<Func<T, bool>> second)
        {
            if (condition)
            {
                if (first == null)
                {
                    if (second == null)
                    {
                        return first;
                    }
                    else
                    {
                        return second;
                    }
                }
                else
                {
                    return first.Compose(second, Expression.OrElse);
                }
            }
            else
            {
                return first;
            }
        }

        private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map(from parameters of second to parameters of first)
            var map = first.Parameters
                .Select((f, i) => new
                {
                    f,
                    s = second.Parameters[i]
                })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression
            var body = merge(first.Body, secondBody);
            return Expression.Lambda<T>(body, first.Parameters);
        }
    }

    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this._map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (_map.TryGetValue(p, out var replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }

    #endregion

    /*
    #region 创建默认实体对象

    public static class ExpressionExtensions
    {
        public static (bool HasValue, TOut Value) CreateDefaultWhenIdFu1<TFunc, TOut>(
            this Expression<Func<TFunc, bool>>[] exp, Action<TOut> action) where TOut : class, new()
        {
            Expression<Func<TFunc, bool>> idEqualExpression = null;
            foreach (Expression<Func<TFunc, bool>> expression in exp)
            {
                dynamic body = expression.Body;
                dynamic left = body.Left;
                string leftMemberName = ExpressionHelper.GetLeftMemberName(left);
                if (string.Compare("Id", leftMemberName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    idEqualExpression = expression;
                    break;
                }
            }

            if (idEqualExpression is null)
            {
                return (false, default(TOut));
            }

            dynamic right = ((dynamic)idEqualExpression.Body).Right;

            dynamic rightValue = ExpressionHelper.GetRightValue(right);
            var valueEqualFu1 = rightValue is int rightValueInt && rightValueInt == -1;

            if (!valueEqualFu1)
            {
                return (false, default(TOut));
            }

            TOut objT = new TOut();
            action?.Invoke(objT);
            //objT = new YPDetailDto();

            //objT.DynamicProperty = JsonConvert.SerializeObject(new
            //{
            //    Length = (string)null,
            //    Width = (string)null,
            //    Thickness = (string)null,
            //});

            //foreach (var propertyInfo in objT.GetType().GetProperties())
            //{
            //    if (propertyInfo.PropertyType == typeof(DateTime))
            //    {
            //        propertyInfo.SetValue(objT, DateTime.Now);
            //    }
            //}
            CreateDefault(objT, objT.GetType());
            return (true, objT);
        }

        private static void CreateDefault(object obj, Type type)
        {
            var typeString = typeof(string);
            var typeDateTime = typeof(DateTime);
            var rnd = new Random();

            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.PropertyType == typeDateTime)
                {
                    propertyInfo.SetValue(obj, DateTime.Now);
                }
                else if (propertyInfo.PropertyType.IsClass)
                {
                    if (propertyInfo.PropertyType == typeString)
                    {
                        //有 MysqlColumnsAttribute || StringLengthAttribute 的才会随机赋值
                        bool randomSetValue = false;
                        int len = 0;
                        if (!randomSetValue)
                        {
                            var propAttr =
                                ReflectionHelper.GetAttributeForProperty<MysqlColumnsAttribute>(propertyInfo);
                            if (propAttr.ContainsKey(propertyInfo) && propAttr[propertyInfo].Length == 1)
                            {
                                randomSetValue = true;
                                var attr = (MysqlColumnsAttribute)propAttr[propertyInfo][0];
                                if (!attr.CHARACTER_CanBeNull)
                                {
                                    //1-4的随机长度
                                    len = rnd.Next(1, 4);
                                    //len 与 attr.CHARACTER_MAXIMUM_LENGTH 对比, 取小的那个值
                                    if (len > attr.CHARACTER_MAXIMUM_LENGTH)
                                    {
                                        len = attr.CHARACTER_MAXIMUM_LENGTH;
                                    }
                                }
                            }
                        }

                        if (!randomSetValue)
                        {
                            var propAttr =
                                ReflectionHelper.GetAttributeForProperty<StringLengthAttribute>(propertyInfo);
                            if (propAttr.ContainsKey(propertyInfo) && propAttr[propertyInfo].Length == 1)
                            {
                                randomSetValue = true;
                                var attr = (StringLengthAttribute)propAttr[propertyInfo][0];
                                len = rnd.Next(1, 4);
                                if (len > attr.MaximumLength)
                                {
                                    len = attr.MaximumLength;
                                }
                            }
                        }

                        if (randomSetValue)
                        {
                            propertyInfo.SetValue(obj,
                                len <= 0 ? string.Empty : Guid.NewGuid().ToString("N").Substring(0, len));
                        }
                    }
                    else
                    {
                        object propertyObj = Activator.CreateInstance(propertyInfo.PropertyType);
                        propertyInfo.SetValue(obj, propertyObj);
                        var isListObj = typeof(IList).IsAssignableFrom(propertyInfo.PropertyType);
                        if (isListObj)
                        {
                            //遇到List<T>对象,创建一个T对象,添加到List<T>里
                            if (propertyObj is IList listobj)
                            {
                                var listItemType = propertyInfo.PropertyType.GetProperties()
                                    .Where(a => a.Name == "Item").FirstOrDefault()?.PropertyType;
                                if (listItemType != null)
                                {
                                    var listItem = Activator.CreateInstance(listItemType);
                                    CreateDefault(listItem, listItemType);
                                    listobj.Add(listItem);
                                }
                            }
                        }
                        else if (!propertyInfo.PropertyType.IsPrimitive)
                        {
                            //注:这个if成立时, 上面的if不成立,反之,上面的if成立, 这个if也肯定成立
                            CreateDefault(propertyObj, propertyInfo.PropertyType);
                        }
                    }
                }
            }
        }
    }
    #endregion
    */
}

#pragma warning restore IDE0130