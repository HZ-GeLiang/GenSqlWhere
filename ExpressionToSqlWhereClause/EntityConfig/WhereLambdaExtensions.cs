using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.EntityConfig
{
    public static class WhereLambdaExtensions
    {
        internal static List<Expression<Func<TEntity, bool>>> AddRange<TEntity, TSearchModel>(
            this List<Expression<Func<TEntity, bool>>> whereLambdas,
            TSearchModel searchModel, SearchType searchType, List<string> props)
        {
            List<Expression<Func<TEntity, bool>>> expressionList = searchType switch
            {
                SearchType.none => throw new ExpressionToSqlWhereClauseException($"未指定{nameof(searchType)}", (System.Exception)new ArgumentException()),
                SearchType.like => WhereLambdaHelper.AddLike<TEntity, TSearchModel>(searchModel, props),
                SearchType.likeLeft => WhereLambdaHelper.AddLikeLeft<TEntity, TSearchModel>(searchModel, props),
                SearchType.likeRight => WhereLambdaHelper.AddLikeRight<TEntity, TSearchModel>(searchModel, props),
                SearchType.eq => WhereLambdaHelper.AddEqual<TEntity, TSearchModel>(searchModel, props),
                SearchType.neq => WhereLambdaHelper.AddNotEqual<TEntity, TSearchModel>(searchModel, props),
                SearchType.@in => WhereLambdaHelper.AddIn<TEntity, TSearchModel>(searchModel, props),
                SearchType.timeRange => WhereLambdaHelper.AddTimeRange<TEntity, TSearchModel>(searchModel, props),
                SearchType.numberRange => WhereLambdaHelper.AddNumberRange<TEntity, TSearchModel>(searchModel, props),
                SearchType.gt => WhereLambdaHelper.AddGt<TEntity, TSearchModel>(searchModel, props),
                SearchType.ge => WhereLambdaHelper.AddGe<TEntity, TSearchModel>(searchModel, props),
                SearchType.lt => WhereLambdaHelper.AddLt<TEntity, TSearchModel>(searchModel, props),
                SearchType.le => WhereLambdaHelper.AddLe<TEntity, TSearchModel>(searchModel, props),
                _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null),
            };

            whereLambdas.AddRange(expressionList);

            return whereLambdas;
        }

        public static WhereLambda<TEntity, TSearchModel> CrateWhereLambda<TEntity, TSearchModel>(this TSearchModel searchModel, Action<TEntity> _)
            where TSearchModel : class
            where TEntity : class
        {
            return new WhereLambda<TEntity, TSearchModel>(searchModel);
        }

        /// <summary>
        /// 适合TEntity ==  TSearchModel,
        /// 这种方式的, 没法用 timeRange
        /// </summary>
        /// <typeparam name="TSearchModel"></typeparam>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static WhereLambda<TSearchModel, TSearchModel> CrateWhereLambda<TSearchModel>(this TSearchModel searchModel) where TSearchModel : class
        {
            return new WhereLambda<TSearchModel, TSearchModel>(searchModel);
        }

    }

    
}