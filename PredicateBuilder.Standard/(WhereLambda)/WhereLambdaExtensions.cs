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
            switch (searchType)
            {
                case SearchType.None:
                    throw new Exception("未指定" + nameof(searchType));
                case SearchType.Like:
                    whereLambdas.AddRange(WhereLambdaHelper.AddLike<TEntity, TSearchModel>(searchModel, props));
                    break;
                case SearchType.Equal:
                    whereLambdas.AddRange(WhereLambdaHelper.AddEqual<TEntity, TSearchModel>(searchModel, props));
                    break;
                case SearchType.In:
                    whereLambdas.AddRange(WhereLambdaHelper.AddIn<TEntity, TSearchModel>(searchModel, props));
                    break;
                case SearchType.DateTimeRange:
                    whereLambdas.AddRange(WhereLambdaHelper.AddDateTimeRange<TEntity, TSearchModel>(searchModel, props));
                    break;
                case SearchType.NumberRange:
                    whereLambdas.AddRange(WhereLambdaHelper.AddNumberRange<TEntity, TSearchModel>(searchModel, props));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
            }

            return whereLambdas;
        }
    }
}