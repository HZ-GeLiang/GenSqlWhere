using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PredicateBuilder.Standard
{
    internal static class WhereLambdaExtensions
    {
        public static List<Expression<Func<TEntity, bool>>> AddRange<TEntity, TSearchModel>(
            this List<Expression<Func<TEntity, bool>>> whereLambdas,
            TSearchModel searchModel, SearchType searchType, List<string> props)
        {
            List<Expression<Func<TEntity, bool>>> expressionList = null;
            switch (searchType)
            {
                case SearchType.none:
                    throw new Exception("未指定" + nameof(searchType));
                case SearchType.like:
                    expressionList = WhereLambdaHelper.AddLike<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.eq:
                    expressionList = WhereLambdaHelper.AddEqual<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.@in:
                    expressionList = WhereLambdaHelper.AddIn<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.datetimeRange:
                    expressionList = WhereLambdaHelper.AddDateTimeRange<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.numberRange:
                    expressionList = WhereLambdaHelper.AddNumberRange<TEntity, TSearchModel>(searchModel, props);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
            }

            whereLambdas.AddRange(expressionList);

            return whereLambdas;
        }
    }
}