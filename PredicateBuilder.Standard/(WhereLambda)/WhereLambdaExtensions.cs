using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PredicateBuilder.Standard
{
    public static class WhereLambdaExtensions
    {
        internal static List<Expression<Func<TEntity, bool>>> AddRange<TEntity, TSearchModel>(
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
                case SearchType.likeLeft:
                    expressionList = WhereLambdaHelper.AddLikeLeft<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.likeRight:
                    expressionList = WhereLambdaHelper.AddLikeRight<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.eq:
                    expressionList = WhereLambdaHelper.AddEqual<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.neq:
                    expressionList = WhereLambdaHelper.AddNotEqual<TEntity, TSearchModel>(searchModel, props);
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
                case SearchType.gt:
                    expressionList = WhereLambdaHelper.AddGt<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.ge:
                    expressionList = WhereLambdaHelper.AddGe<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.lt:
                    expressionList = WhereLambdaHelper.AddLt<TEntity, TSearchModel>(searchModel, props);
                    break;
                case SearchType.le:
                    expressionList = WhereLambdaHelper.AddLe<TEntity, TSearchModel>(searchModel, props);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
            }

            whereLambdas.AddRange(expressionList);

            return whereLambdas;
        }

        public static WhereLambda<TEntity, TSearchModel> CrateWhereLambda<TSearchModel, TEntity>(this TSearchModel searchModel, Action<TEntity> _)
        {
            return new WhereLambda<TEntity, TSearchModel>(searchModel);
        }
    }
}